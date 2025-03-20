using System;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PropertyMustBeOfSupportedTypeAnalyser() : ConcurrentAnalyzer([Rules.MustBeSupportedType])
{
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
            return;

        var properties = typeSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(property => !IsSupported(property, context.SemanticModel));

        foreach (var property in properties)
            context.ReportDiagnostic(Diagnostic.Create(Rules.MustBeSupportedType, property.GetLocation()));
    }

    private static bool IsSupported(PropertyDeclarationSyntax property, SemanticModel semanticModel)
    {
        if (SupportedTypes.Simple.Supports(semanticModel.GetDeclaredSymbol(property))) return true;
        return SupportedTypes.Collection.Supports(property.Type, semanticModel);
    }
}


