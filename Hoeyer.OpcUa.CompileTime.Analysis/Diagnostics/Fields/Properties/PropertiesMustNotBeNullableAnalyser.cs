using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Diagnostics.Fields.Properties;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PropertiesMustNotBeNullableAnalyser : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(OpcUaDiagnostics.MustNotBeNullablePropertyDescriptor);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        var properties = context.GetOpcEntityPropertyDeclarations()
            .Where(e => e.Type is NullableTypeSyntax)
            .Select(OpcUaDiagnostics.MustNotBeNullableProperty);

        foreach (var diagnostic in properties) context.ReportDiagnostic(diagnostic);
    }
}