using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

public static class SupportedTypes
{
    public static bool IsSupportedTask(TypeSyntax syntax, SemanticModel model)
    {
        var taskType = model.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        var taskOfTType = model.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");

        ITypeSymbol? typeSymbol = model.GetTypeInfo(syntax).Type;
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol) return false;
        
        if (SymbolEqualityComparer.Default.Equals(namedTypeSymbol, taskType) ) return true;
        
        if (!SymbolEqualityComparer.Default.Equals(namedTypeSymbol.OriginalDefinition, taskOfTType)) return false;

        ITypeSymbol typeArg = namedTypeSymbol.TypeArguments.First();
        return Simple.Supports(typeArg);
    }

    public static bool IsSupported(TypeSyntax syntax, SemanticModel model)
    {
        var symbol = model.GetTypeInfo(syntax).Type;
        if (symbol == null)
        {
            return false;
        }
        if (symbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            var s = UnwrapNullable(symbol);
            return Simple.Supports(s) || Collection.Supports(s);
        }
        
        return Simple.Supports(symbol) || Collection.Supports(symbol);
    }


    private static ITypeSymbol UnwrapNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { IsReferenceType: true} namedTypeSymbol)
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        if (typeSymbol is INamedTypeSymbol valueTypeSymbol && valueTypeSymbol.Name == "Nullable")
        {
            return valueTypeSymbol.TypeArguments[0];
        }

        return typeSymbol;
    }

    private static class Simple
    {
        private static readonly ImmutableHashSet<SpecialType> SpecialTypes =
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

    /// <summary>
    /// Currently supported types are: generic lists
    /// </summary>
    private static class Collection
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