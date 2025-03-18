using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;

public class OpcPropertyTypeInfoFactory(PropertyDeclarationSyntax property, SemanticModel semanticModel)
{
    private const string DATA_TYPE_ENUM_NAME = nameof(DataTypeIds);
    private const string VALUE_RANK_ENUM_NAME = nameof(ValueRanks);
    private const string VALUE_RANK_SINGLE_VALUE = VALUE_RANK_ENUM_NAME + "." + nameof(ValueRanks.Scalar);
    private const string VALUE_RANK_ONE_DIM = VALUE_RANK_ENUM_NAME + "." + nameof(ValueRanks.OneDimension);

    private const string OPC_UA_BOOLEAN_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Boolean);
    private const string OPC_UA_BYTE_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Byte);
    private const string OPC_UA_INT16_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Int16);
    private const string OPC_UA_U_INT16_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.UInt16);
    private const string OPC_UA_INT32_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Int32);
    private const string OPC_UA_U_INT32_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.UInt32);
    private const string OPC_UA_INT64_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Int64);
    private const string OPC_UA_U_INT64_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.UInt64);
    private const string OPC_UA_FLOAT_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Float);
    private const string OPC_UA_DOUBLE_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.Double);
    private const string OPC_UA_STRING_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.String);
    private const string OPC_UA_DATE_TIME_TYPE = DATA_TYPE_ENUM_NAME + "." + nameof(DataTypeIds.DateTime);

    private static readonly ImmutableHashSet<string> SupportedEnumerableNames = ImmutableHashSet.CreateRange
    ([
        "IList<>",
        "ICollection<>",
        "IEnumerable<>",
        "List<>",
        "ISet<>",
        "HashSet<>",
        "SortedSet<>",
        "SortedList<>"
    ]);

    private static readonly Dictionary<SpecialType, string> SpecialTypeAndEquivalentOpcTypes = new()
    {
        { SpecialType.System_Boolean, OPC_UA_BOOLEAN_TYPE },
        { SpecialType.System_Byte, OPC_UA_BYTE_TYPE },
        { SpecialType.System_Int16, OPC_UA_INT16_TYPE },
        { SpecialType.System_UInt16, OPC_UA_U_INT16_TYPE },
        { SpecialType.System_Int32, OPC_UA_INT32_TYPE },
        { SpecialType.System_UInt32, OPC_UA_U_INT32_TYPE },
        { SpecialType.System_Int64, OPC_UA_INT64_TYPE },
        { SpecialType.System_UInt64, OPC_UA_U_INT64_TYPE },
        { SpecialType.System_Single, OPC_UA_FLOAT_TYPE },
        { SpecialType.System_Double, OPC_UA_DOUBLE_TYPE },
        { SpecialType.System_String, OPC_UA_STRING_TYPE },
        { SpecialType.System_DateTime, OPC_UA_DATE_TIME_TYPE },
    };

    private static readonly Dictionary<SyntaxKind, string> SyntaxKindsAndEquivalentOpcTypes = new()
    {
        { SyntaxKind.BoolKeyword, OPC_UA_BOOLEAN_TYPE },
        { SyntaxKind.ByteKeyword, OPC_UA_BYTE_TYPE },
        { SyntaxKind.ShortKeyword, OPC_UA_INT16_TYPE },
        { SyntaxKind.UShortKeyword, OPC_UA_U_INT16_TYPE },
        { SyntaxKind.IntKeyword, OPC_UA_INT32_TYPE },
        { SyntaxKind.UIntKeyword, OPC_UA_U_INT32_TYPE },
        { SyntaxKind.LongKeyword, OPC_UA_INT64_TYPE },
        { SyntaxKind.ULongKeyword, OPC_UA_U_INT64_TYPE },
        { SyntaxKind.FloatKeyword, OPC_UA_FLOAT_TYPE },
        { SyntaxKind.DoubleKeyword, OPC_UA_DOUBLE_TYPE },
        { SyntaxKind.StringKeyword, OPC_UA_STRING_TYPE },
    };

    private static readonly ImmutableHashSet<SyntaxKind> SupportedSimpleTypesSyntaxKind =
        ImmutableHashSet.CreateRange(SyntaxKindsAndEquivalentOpcTypes.Keys);

    private static readonly ImmutableHashSet<SpecialType> SupportedSimpleSpecialtypes =
        ImmutableHashSet.CreateRange(SpecialTypeAndEquivalentOpcTypes.Keys);


    private (string SimpleType, string OpcType, string ValueRank)? FindSupportedType()
    {
        var typeSyntax = property.Type;
        var syntaxKind = typeSyntax.Kind();
        if (SupportedSimpleTypesSyntaxKind.Contains(syntaxKind))
            return (
                typeSyntax.ToFullString(),
                SyntaxKindsAndEquivalentOpcTypes[syntaxKind],
                VALUE_RANK_SINGLE_VALUE);

        var typeInfo = semanticModel.GetTypeInfo(typeSyntax).Type;
        if (typeInfo == null) return null;

        if (SupportedSimpleSpecialtypes.Contains(typeInfo.SpecialType))
            return (typeInfo.ToString(),
                SpecialTypeAndEquivalentOpcTypes[typeInfo.SpecialType],
                VALUE_RANK_SINGLE_VALUE);


        if (typeInfo is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedTypeSymbol)
        {
            var span = namedTypeSymbol.ConstructUnboundGenericType().ToString().AsSpan();
            var lastDot = span.LastIndexOf('.') + 1; //even if no . then it returns index 0! :-)

            var startIndex = span.Slice(lastDot).IndexOf('<');
            var endIndex = span.IndexOf('>');
            var upToBracket = span.Slice(lastDot, startIndex + 1);
            var skippedTypeArgument = span.Slice(endIndex);

            var collectionTypeGenericName = new ReadOnlySpan<char>([..upToBracket, ..skippedTypeArgument]);


            if (TryGetSupportedParam(collectionTypeGenericName.ToString(), namedTypeSymbol, out var typeArgument))
                return (typeArgument.ToDisplayString(),
                    SpecialTypeAndEquivalentOpcTypes[typeArgument.SpecialType],
                    VALUE_RANK_ONE_DIM);
        }

        return null;
    }

    private static bool TryGetSupportedParam(string genericTypeName, INamedTypeSymbol namedTypeSymbol,
        out ITypeSymbol genericParam)
    {
        genericParam = namedTypeSymbol.TypeArguments[0];
        return SupportedEnumerableNames.Contains(genericTypeName)
               && SupportedSimpleSpecialtypes.Contains(genericParam.SpecialType);
    }


    public (OpcUaProperty PropertyInfo, bool TypeIsSupported, PropertyDeclarationSyntax PropertyDecleration)
        GetTypeInfo()
    {
        var typeDetails = FindSupportedType();
        if (typeDetails == null) return (new OpcUaProperty(), false, property);

        return
        (
            PropertyInfo: new OpcUaProperty(
                property.Identifier.ToFullString(),
                typeDetails.Value.SimpleType,
                typeDetails.Value.OpcType,
                typeDetails.Value.ValueRank
            ),
            TypeIsSupported: true,
            PropertyDecleration: property);
    }
}