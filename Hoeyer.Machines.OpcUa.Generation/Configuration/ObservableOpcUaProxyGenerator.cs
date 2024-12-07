using System.Collections.Generic;
using System.Linq;
using Hoeyer.Machines.OpcUa.Extensions;
using Hoeyer.Machines.OpcUa.Generated.Configuration;
using Hoeyer.Machines.OpcUa.ResourceLoading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.Machines.OpcUa.Configuration;

[Generator]
internal class ObservableOpcUaProxyGenerator : IIncrementalGenerator
{
    private const string ATTRIBUTE_META_NAME = $"Hoeyer.Machines.OpcUa.Generated.Configuration.{nameof(OpcNodeConfigurationAttribute)}";

    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = GetValueProviderForDecorated<BaseTypeDeclarationSyntax>(context, ATTRIBUTE_META_NAME);
        context.RegisterSourceOutput(decoratedRecordsProvider, (sourceProductionContext, typeContext) =>
        {
            var templateInformation = new CodeTemplateInformation (
                    TemplateClassName: "ObservableMachineProxy",
                    TemplateClassResourcePath: "Hoeyer.Machines.OpcUa.Generated.Machine.ObservableMachineProxy"
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


    private static bool IsPublicReadableField(MemberDeclarationSyntax m)
    {
        return m is PropertyDeclarationSyntax property && property.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword)) 
               || m is FieldDeclarationSyntax field && field.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword));
    }
}