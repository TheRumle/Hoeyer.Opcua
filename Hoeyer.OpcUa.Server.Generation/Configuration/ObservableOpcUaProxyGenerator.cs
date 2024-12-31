using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.Configuration;

[Generator]
internal class ObservableOpcUaProxyGenerator : IIncrementalGenerator
{
    private const string ATTRIBUTE_META_NAME = "Hoeyer.OpcUa.OpcUaEntity";


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context.SyntaxProvider.ForAttributeWithMetadataName(ATTRIBUTE_META_NAME,
            (decoratedClass, cancellationToken) => decoratedClass is ClassDeclarationSyntax,
            (attributeSyntaxContext, cancellationToken) => attributeSyntaxContext);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider, static (context, a) =>
        {
            try
            {
                // Your logic here
                Console.WriteLine("Processing implementation source output.");
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "GEN003",
                        "Generator Error",
                        "Error occurred: {0}",
                        "Generator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None,
                    ex.Message));
            }
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "GEN002",
                    "Generator Debugging",
                    "Found a class with the attribute: {0}",
                    "Generator",
                    DiagnosticSeverity.Info,
                    isEnabledByDefault: true),
                Location.None,
                a.TargetSymbol.ToDisplayString()));
        });
    }
}