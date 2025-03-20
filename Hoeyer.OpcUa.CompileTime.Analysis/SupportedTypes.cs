using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

public static class SupportedTypes
{
    public static class Simple
    {
        public static readonly ImmutableHashSet<SpecialType> SpecialTypes =
        [
            SpecialType.System_Boolean,
            SpecialType.System_Byte,
            SpecialType.System_Int16,
            SpecialType.System_UInt16,
            SpecialType.System_Int32,
            SpecialType.System_UInt32,
            SpecialType.System_Int64,
            SpecialType.System_UInt64,
            SpecialType.System_Single,
            SpecialType.System_Double,
            SpecialType.System_String,
            SpecialType.System_DateTime
        ];

        public static bool Supports(SpecialType specialType) => SpecialTypes.Contains(specialType);

        public static bool Supports(IPropertySymbol? propertySymbol) =>
            propertySymbol switch
            {
                { Type.SpecialType: var specialType } when SpecialTypes.Contains(specialType) => true,
                { Type: { } type } when Supports(type) => true,
                _ => false
            };

        public static bool Supports(ITypeSymbol type)
        {
            if (SpecialTypes.Contains(type.SpecialType)) return true;
            return SupportedSyntaxKinds.Values.Contains(
                type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        public static readonly ImmutableDictionary<SyntaxKind, string> SupportedSyntaxKinds =
            new Dictionary<SyntaxKind, string>
            {
                { SyntaxKind.BoolKeyword, "bool" },
                { SyntaxKind.ByteKeyword, "byte" },
                { SyntaxKind.ShortKeyword, "short" },
                { SyntaxKind.UShortKeyword, "ushort" },
                { SyntaxKind.IntKeyword, "int" },
                { SyntaxKind.UIntKeyword, "uint" },
                { SyntaxKind.LongKeyword, "long" },
                { SyntaxKind.ULongKeyword, "ulong" },
                { SyntaxKind.FloatKeyword, "float" },
                { SyntaxKind.DoubleKeyword, "double" },
                { SyntaxKind.StringKeyword, "string" }
            }.ToImmutableDictionary();

    }

    public static class Collection
    {
        private static readonly ISet<string> SupportedCollectionTypes = new HashSet<string>
        {
            "global::System.Collections.Generic.IList",
            "global::System.Collections.Generic.ICollection",
            "global::System.Collections.Generic.IEnumerable",
            "global::System.Collections.Generic.List",
            "global::System.Collections.Generic.ISet",
            "global::System.Collections.Generic.HashSet",
        }.ToImmutableHashSet();


        public static bool Supports(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol { Arity: 1, IsGenericType: true } namedTypeSymbol)
            {
                var unTypedVersion = namedTypeSymbol.ConstructUnboundGenericType();
                return SupportedCollectionTypes.Contains(unTypedVersion.GloballyQualifiedNonGeneric()) &&
                    Simple.Supports(namedTypeSymbol.TypeArguments.First());
            }

            return false;
        }
    }

    public static bool IsSupported(ITypeSymbol symbol)
    {
        return Simple.Supports(symbol) || Collection.Supports(symbol);
    }
    
    public static bool IsSupported(TypeSyntax syntax, SemanticModel model)
    {
        var symbol = model.GetTypeInfo(syntax).Type;
        if (symbol == null) return false;
        return Simple.Supports(symbol) || Collection.Supports(symbol);
    }
}