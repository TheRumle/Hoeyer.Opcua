using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool IsAnnotatedAsOpcUaEntity(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        if (typeSyntax.AttributeLists.Count == 0) return false;
        return typeSyntax.AttributeLists.Any(attributes =>
            attributes.Attributes.Any(attribute =>
                nameof(OpcUaEntityAttribute) == semanticModel.GetTypeInfo(attribute)!.Type!.Name));
    }

    public static IEnumerable<PropertyDeclarationSyntax> GetOpcEntityPropertyDeclarations(this SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
            return [];

        return typeSyntax.Members.OfType<PropertyDeclarationSyntax>();
    }
}