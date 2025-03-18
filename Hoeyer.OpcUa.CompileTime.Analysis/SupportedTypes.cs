using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public static bool Supports(ITypeSymbol type) =>
            SupportedSyntaxKinds.Values.Contains(type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

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
        private static readonly ISet<TypeSyntax> SupportedCollectionTypes = new HashSet<string>
        {
            "global::System.Collections.Generic.IList'1",
            "global::System.Collections.Generic.ICollection'1",
            "global::System.Collections.Generic.IEnumerable'1",
            "global::System.Collections.Generic.List'1",
            "global::System.Collections.Generic.ISet'1",
            "global::System.Collections.Generic.HashSet'1",
            "global::System.Collections.Generic.SortedSet'1",
            "global::System.Collections.Generic.SortedList'1"
        }.Select(e => SyntaxFactory.ParseTypeName(e)).ToFrozenSet();


        public static bool Supports(TypeSyntax typeSymbol, SemanticModel model)
        {
            SymbolEqualityComparer comparer = SymbolEqualityComparer.Default;
            return (from supported in SupportedCollectionTypes 
                let actual = model.GetTypeInfo(typeSymbol).Type 
                let expected = model.GetTypeInfo(supported).Type 
                where comparer.Equals(expected, actual) select actual)
                .Any();
        }

    }
}