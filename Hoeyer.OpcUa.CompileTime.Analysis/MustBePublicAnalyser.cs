using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MustBePublicAnalyser() : ConcurrentAnalyzer([Rules.MustHavePublicSetter])
{
    
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyseSymbol, SymbolKind.NamedType);
    }

    private void AnalyseSymbol(SymbolAnalysisContext context)
    {
        // Ensure the symbol is a class or record
        if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class } classDefinition) return;
        if (!classDefinition.IsAnnotatedAsOpcUaEntity()) return;

        var properties = classDefinition
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(e => e.SetMethod is not { DeclaredAccessibility: Accessibility.Public })
            .Select(e => Diagnostic.Create(Rules.MustHavePublicSetter, e.Locations.First()));
        
        foreach (var diagnostic in properties) context.ReportDiagnostic(diagnostic);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
            return;
        

        var properties = typeSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(e => !e.IsFullyPublicProperty())
            .Select(e => Diagnostic.Create(Rules.MustHavePublicSetter, e.GetLocation()));

        foreach (var diagnostic in properties) context.ReportDiagnostic(diagnostic);
    }
}