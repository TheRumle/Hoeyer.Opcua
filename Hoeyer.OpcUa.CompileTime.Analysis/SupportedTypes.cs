using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

public static class SupportedTypes
{
    public static bool IsSupported(ITypeSymbol symbol)
    {
        return Simple.Supports(symbol) || Collection.Supports(symbol);
    }

    public static bool IsSupported(TypeSyntax syntax, SemanticModel model)
    {
        var symbol = model.GetTypeInfo(syntax).Type;
        if (symbol == null)
        {
            return false;
        }

        return Simple.Supports(symbol) || Collection.Supports(symbol);
    }

    public static class Simple
    {
        public static readonly ImmutableHashSet<SpecialType> SpecialTypes =
        [
            SpecialType.System_Enum,
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

     

        public static bool Supports(SpecialType specialType)
        {
            return SpecialTypes.Contains(specialType);
        }

        public static bool Supports(IPropertySymbol? propertySymbol)
        {
            return propertySymbol switch
            {
                { Type.SpecialType: var specialType } when SpecialTypes.Contains(specialType) => true,
                { Type: { } type } when Supports(type) => true,
                _ => false
            };
        }

        public static bool Supports(ITypeSymbol type)
        {
            if (SpecialTypes.Contains(type.SpecialType))
            {
                return true;
            }

            if (type.TypeKind == TypeKind.Enum) return true;

            return false;
        }
    }

    public static class Collection
    {
        public static bool Supports(ITypeSymbol typeSymbol)
        {
            var implementsICollection = typeSymbol
                .AllInterfaces
                .Any(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IList_T);

            if (typeSymbol is INamedTypeSymbol { Arity: 1, IsGenericType: true } namedTypeSymbol)
            {
                return Simple.Supports(namedTypeSymbol.TypeArguments.First())
                       && implementsICollection
                       && namedTypeSymbol.Constructors.Any(c =>
                           c.Parameters.Length == 0 && // Check for no parameters
                           c.DeclaredAccessibility == Accessibility.Public);
            }

            return false;
        }
    }
}