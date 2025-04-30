using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PropertiesMustNotBeNullableAnalyser() : ConcurrentAnalyzer([Rules.MustNotBeNullablePropertyDescriptor])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        var properties = context.GetOpcEntityPropertyDeclarations()
            .Where(e => e.Type is NullableTypeSyntax)
            .Select(Rules.MustNotBeNullableProperty);

        foreach (var diagnostic in properties) context.ReportDiagnostic(diagnostic);
    }
}