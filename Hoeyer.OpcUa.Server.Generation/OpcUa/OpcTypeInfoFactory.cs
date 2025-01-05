using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Generation.OpcUa;

public class OpcTypeInfoFactory(PropertyDeclarationSyntax property, SemanticModel semanticModel)
{
    private const string DATA_TYPE_ENUM_NAME = nameof(DataTypes);
    private const string VALUE_RANK_ENUM_NAME = nameof(ValueRanks);

    private static readonly ImmutableHashSet<string> SUPPORTED_ENUMERABLE_NAMES = ImmutableHashSet.CreateRange
    ([
        "System.Collections.Generic.IList<>", "System.Collections.Generic.ICollection<>",
        "System.Collections.Generic.IEnumerable<>", "System.Collections.Generic.List<>",
        "System.Collections.Generic.ISet<>",
        "System.Collections.Generic.HashSet<>", "System.Collections.Generic.SortedSet<>",
        "System.Collections.Generic.SortedList<>"
    ]);
    
    private const string OpcUaBooleanType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Boolean);
    private const string OpcUaByteType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Byte);
    private const string OpcUaInt16Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Int16);
    private const string OpcUaUInt16Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.UInt16);
    private const string OpcUaInt32Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Int32);
    private const string OpcUaUInt32Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.UInt32);
    private const string OpcUaInt64Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Int64);
    private const string OpcUaUInt64Type =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.UInt64);
    private const string OpcUaFloatType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Float);
    private const string OpcUaDoubleType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Double);
    private const string OpcUaStringType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.String);
    private const string OpcUaDateTimeType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.DateTime);
    private const string OpcUaDecimalType =  DATA_TYPE_ENUM_NAME + "." + nameof(DataTypes.Decimal);
    
    private static readonly Dictionary<SpecialType, string> SPECIAL_TYPE_OPC_NATIVE_TYPES = new(){
        {SpecialType.System_Boolean, OpcUaBooleanType},
        {SpecialType.System_Byte, OpcUaByteType},
        {SpecialType.System_Int16, OpcUaInt16Type},
        {SpecialType.System_UInt16, OpcUaUInt16Type},
        {SpecialType.System_Int32, OpcUaInt32Type},
        {SpecialType.System_UInt32, OpcUaUInt32Type},
        {SpecialType.System_Int64, OpcUaInt64Type},
        {SpecialType.System_UInt64, OpcUaUInt64Type},
        {SpecialType.System_Single, OpcUaFloatType},
        {SpecialType.System_Double, OpcUaDoubleType},
        {SpecialType.System_String, OpcUaStringType},
        {SpecialType.System_DateTime, OpcUaDateTimeType},
        {SpecialType.System_Decimal, OpcUaDecimalType}
    };
    
    private static readonly Dictionary<SyntaxKind, string> OPC_NATIVE_TYPES = new(){
        {SyntaxKind.BoolKeyword, OpcUaBooleanType},
        {SyntaxKind.ByteKeyword, OpcUaByteType},
        {SyntaxKind.ShortKeyword, OpcUaInt16Type},
        {SyntaxKind.UShortKeyword, OpcUaUInt16Type},
        {SyntaxKind.IntKeyword, OpcUaInt32Type},
        {SyntaxKind.UIntKeyword, OpcUaUInt32Type},
        {SyntaxKind.LongKeyword, OpcUaInt64Type},
        {SyntaxKind.ULongKeyword, OpcUaUInt64Type},
        {SyntaxKind.FloatKeyword, OpcUaFloatType},
        {SyntaxKind.DoubleKeyword, OpcUaDoubleType},
        {SyntaxKind.StringKeyword, OpcUaStringType},
        {SyntaxKind.DecimalKeyword, OpcUaDateTimeType}
    };

    private static readonly ImmutableHashSet<SyntaxKind> SUPPORTED_SIMPLE_TYPES_SYNTAX_KIND = ImmutableHashSet.CreateRange(OPC_NATIVE_TYPES.Keys);

    private static readonly ImmutableHashSet<SpecialType> SUPPORTED_SIMPLE_SPECIALTYPES = ImmutableHashSet.CreateRange (SPECIAL_TYPE_OPC_NATIVE_TYPES.Keys);
    

    private (string SimpleType, string OpcType, string ValueRank)? FindSupportedTypes()
    {
        var typeSyntax = property.Type;
        var syntaxKind = typeSyntax.Kind();
        if (SUPPORTED_SIMPLE_TYPES_SYNTAX_KIND.Contains(syntaxKind))
        {
            return (
                typeSyntax.ToFullString(),
                OPC_NATIVE_TYPES[syntaxKind],
                VALUE_RANK_ENUM_NAME + nameof(ValueRanks.Scalar));
        }

        var typeInfo = semanticModel.GetTypeInfo(property.Type).Type;
        if (typeInfo == null) return null;
        
        if (SUPPORTED_SIMPLE_SPECIALTYPES.Contains(typeInfo.SpecialType))
        {
            return (typeInfo.ToString(),
                SPECIAL_TYPE_OPC_NATIVE_TYPES[typeInfo.SpecialType],
                VALUE_RANK_ENUM_NAME + nameof(ValueRanks.Scalar));
        }
        
        

        if (typeInfo is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedTypeSymbol )
        {
            var collectionTypeName = namedTypeSymbol.ConstructUnboundGenericType().ToDisplayString();
            if (TryGetSupportedParam(collectionTypeName, namedTypeSymbol, out var typeArgument))
            {
                return (typeArgument.ToDisplayString(),
                    SPECIAL_TYPE_OPC_NATIVE_TYPES[typeArgument.SpecialType],
                    VALUE_RANK_ENUM_NAME + nameof(ValueRanks.OneDimension));
            }
        }

        return null;
    }

    private static bool TryGetSupportedParam(string genericTypeName, INamedTypeSymbol namedTypeSymbol, out ITypeSymbol genericParam)
    {
        genericParam = namedTypeSymbol.TypeArguments[0];
        return SUPPORTED_ENUMERABLE_NAMES.Contains(genericTypeName) 
               && SUPPORTED_SIMPLE_SPECIALTYPES.Contains(genericParam.SpecialType);
    }


    public (OpcUaProperty PropertyInfo, bool TypeIsSupported, PropertyDeclarationSyntax PropertyDecleration) GetTypeInfo()
    {
        var typeDetails = FindSupportedTypes();
        if (typeDetails == null) return (new OpcUaProperty(), false, property);
        
        return 
            (
                PropertyInfo: new OpcUaProperty (
                    Name: property.Identifier.ToFullString(),
                    Type: typeDetails.Value.SimpleType,
                    OpcUaTypeId: typeDetails.Value.OpcType,
                    ValueRank: typeDetails.Value.ValueRank
                ), 
            TypeIsSupported: true, 
            PropertyDecleration: property);
    }


}