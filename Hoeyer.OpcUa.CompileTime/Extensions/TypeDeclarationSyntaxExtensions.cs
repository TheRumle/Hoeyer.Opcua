using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Extensions;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        if (typeSyntax.AttributeLists.Count == 0) return false;
        return typeSyntax.AttributeLists.Any(attributes =>
            attributes.Attributes.Any(attribute =>
                ModellingNamespace.ENTITY_ATTRIBUTE_FULLNAME.Equals(attribute.FullyQualifiedName(semanticModel))));
    }
}