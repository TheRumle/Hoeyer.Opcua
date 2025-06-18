using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public sealed class InterfaceUsageReporter
{
    private readonly SyntaxNodeAnalysisContext _context;
    private readonly Location _location;

    public InterfaceUsageReporter(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration,
        INamedTypeSymbol violatedInterfaceSymbol)
    {
        _context = context;
        _location = typeDeclaration
            .BaseList!
            .Types
            .First(baseType =>
            {
                ITypeSymbol? actualType = context.SemanticModel.GetTypeInfo(baseType.Type).Type;
                return actualType is INamedTypeSymbol named &&
                       SymbolEqualityComparer.Default.Equals(named, violatedInterfaceSymbol);
            }).GetLocation();
    }

    public void ReportDiagnostic(DiagnosticDescriptor descriptor, params object[] arguments)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, _location, arguments));
    }

    public void ReportMethodMismatchDiagnostic(IMethodSymbol symbol, DiagnosticDescriptor descriptor,
        params object[] arguments)
    {
        _context.ReportDiagnostic(Diagnostic.Create(descriptor, _location, arguments));
    }
}