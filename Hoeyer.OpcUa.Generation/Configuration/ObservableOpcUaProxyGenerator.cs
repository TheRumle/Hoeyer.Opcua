using Hoeyer.Machines.OpcUa.Extensions;
using Hoeyer.Machines.OpcUa.ResourceLoading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.Machines.OpcUa.Configuration;

[Generator]
internal class ObservableOpcUaProxyGenerator : IIncrementalGenerator
{
    private const string ATTRIBUTE_META_NAME = $"Hoeyer.OpcUa.OpcUa.Client.Configuration.{nameof(OpcUaEntity)}";

    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = GetValueProviderForDecorated<BaseTypeDeclarationSyntax>(context, ATTRIBUTE_META_NAME);
        context.RegisterSourceOutput(decoratedRecordsProvider, (sourceProductionContext, typeContext) =>
        {
            var templateInformation = new CodeTemplateInformation (
                    TemplateClassName: "MachineObserver",
                    TemplateClassResourcePath: "Hoeyer.OpcUa.OpcUa.Client.Generated.Observation.MachineObserver"
            );
            
            var loader = new CSharpTemplateFileLoader(new TemplateFileLoad(templateInformation, typeContext), sourceProductionContext.ReportDiagnostic);
            foreach (var loadableType in  loader.LoadResources())
            {
                sourceProductionContext.Load(loadableType);
            }
        });

    }

    private static IncrementalValuesProvider<TypeContext> GetValueProviderForDecorated<T>(IncrementalGeneratorInitializationContext context, string attributeMetaName)
    where T : BaseTypeDeclarationSyntax
    {
        return context.SyntaxProvider.ForAttributeWithMetadataName(attributeMetaName,
            predicate: (decoratedClass, cancellationToken) => decoratedClass is T,
            transform: (attributeSyntaxContext, cancellationToken) =>
                new TypeContext(attributeSyntaxContext.SemanticModel, (T)attributeSyntaxContext.TargetNode));

    }
}