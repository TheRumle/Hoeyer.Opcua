using System;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.SourceGeneration.Generation.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration;

[Generator]
public class RemoteMethodCallerGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        UnloadedIncrementalValuesProvider<string> decoratedRecordsProvider = context
            .GetEntityMethodInterfaces()
            .Select(async (typeContext, cancellationToken) =>
                await CreateCompilationUnit(typeContext, cancellationToken))
            .Select((e, _) => e.Result);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(),
            (productionContext, compilations) => { });
    }

    private static async Task<string> CreateCompilationUnit(
        TypeContext<InterfaceDeclarationSyntax> typeContext, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}