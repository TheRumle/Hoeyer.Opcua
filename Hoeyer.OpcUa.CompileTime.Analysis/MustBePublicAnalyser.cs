using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
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

    private static void AnalyseSymbol(SymbolAnalysisContext context)
    {
        // Ensure the symbol is a class or record
        if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Class } typeDefinition) return;
        if (!typeDefinition.IsAnnotatedAsOpcUaEntity()) return;

        var properties = typeDefinition
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(e => e.SetMethod is null // No setter at all (read-only property)
                        || e.SetMethod.DeclaredAccessibility != Accessibility.Public
                        || e.SetMethod.Name == ""
                        || e.SetMethod.IsInitOnly)
            .Select(e => Diagnostic.Create(Rules.MustHavePublicSetter, e.Locations.First()));

        foreach (var diagnostic in properties) context.ReportDiagnostic(diagnostic);
    }
}