using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Generation.IncrementalProvider;

internal static class IncrementalGeneratorExtensions
{
    public static UnloadedIncrementalValuesProvider<(InterfaceDeclarationSyntax interfaceNode, SemanticModel model)>
        GetAgentMethodInterfaces(
            this IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<GeneratorSyntaxContext> interfaceDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is InterfaceDeclarationSyntax { AttributeLists.Count: > 0 },
                static (ctx, _) => ctx);


        IncrementalValuesProvider<(InterfaceDeclarationSyntax interfaceNode, SemanticModel model)> annotatedInterfaces =
            interfaceDeclarations
                .Select((contextTuple, _) =>
                {
                    (var interfaceNode, SemanticModel model) =
                        ((InterfaceDeclarationSyntax)contextTuple.Node, contextTuple.SemanticModel);
                    ISymbol? symbol = model.GetDeclaredSymbol(interfaceNode);
                    if (symbol is null) return default;

                    foreach (INamedTypeSymbol? attributeClass in symbol.GetAttributes()
                                 .Select(attributeData => attributeData.AttributeClass))
                    {
                        if (attributeClass is null) continue;


                        if (attributeClass.Name == "OpcUaAgentMethodsAttribute" &&
                            attributeClass.ContainingNamespace.ToDisplayString() == "Hoeyer.OpcUa.Core" &&
                            attributeClass.IsGenericType)
                            return (interfaceNode, model);
                    }

                    return default;
                })
                .Where(e => e != default);

        return new UnloadedIncrementalValuesProvider<(InterfaceDeclarationSyntax interfaceNode, SemanticModel model)>(
            annotatedInterfaces);
    }
}