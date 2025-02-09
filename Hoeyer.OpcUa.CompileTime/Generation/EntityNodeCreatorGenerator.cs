using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Extensions;
using Hoeyer.OpcUa.CompileTime.Generation.IncrementalProvider;
using Hoeyer.OpcUa.CompileTime.OpcUaTypes;
using Hoeyer.OpcUa.EntityGeneration.IncrementalProvider;
using Hoeyer.OpcUa.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.CompileTime.Generation;

[Generator]
public class EntityNodeCreatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeDeclarationsDecoratedWith<ClassDeclarationSyntax>(ModellingNamespace.ENTITY_ATTRIBUTE_FULLNAME)
            .Select((e, c) => new TypeContext<ClassDeclarationSyntax>(e.SemanticModel, e.Node));

        context.RegisterImplementationSourceOutput(
            decoratedRecordsProvider.Collect(),
            static (context, typeContexts) =>
            {
                foreach (var typeContext in typeContexts) AddNodeGeneratorSourceCode(typeContext, context);
            });
    }

    private static void AddNodeGeneratorSourceCode(TypeContext<ClassDeclarationSyntax> typeContext,
        SourceProductionContext context)
    {
        var entityName = typeContext.Node.Identifier.ToString();
        var nodeName = entityName + "Node";
        var className = $"{entityName}OpcUaNodeCreator";
        var sourceCode = CreateSourceCode(typeContext, entityName, nodeName, className);
        context.AddSource(className + ".g.cs", sourceCode);
    }

    private static string CreateSourceCode(TypeContext<ClassDeclarationSyntax> typeContext, string entityName,
        string nodeStateReference, string className)
    {
        return $$"""
                 using System;
                 using System.Linq;
                 using Hoeyer.OpcUa.Nodes;
                 using Opc.Ua;

                 namespace Hoeyer.OpcUa.Server.Entity
                 {
                     internal sealed class {{className}} : {{nameof(IEntityNodeCreator)}}
                     {
                        public {{className}}(){}
                        public string EntityName { get; } = "{{entityName}}";
                        public {{nameof(NodeState)}} CreateEntityOpcUaNode({{nameof(NodeState)}} root, ushort applicationNamespaceIndex)
                        {
                            {{nameof(BaseObjectState)}} {{nodeStateReference}} = new {{nameof(BaseObjectState)}}(root)
                            {
                                BrowseName =  new {{nameof(QualifiedName)}}("{{entityName}}", applicationNamespaceIndex),
                                NodeId = new {{nameof(NodeId)}}({{nameof(Guid)}}.{{nameof(Guid.NewGuid)}}(), applicationNamespaceIndex),
                                DisplayName = "{{entityName}}",
                            };
                        
                            root.AddChild({{nodeStateReference}});
                            
                            //Assign properties
                            {{string.Join("\n", GetPropertyCreationStatements(typeContext, nodeStateReference))}}
                            
                            return {{nodeStateReference}};
                        }
                     }
                 }
                 """;
    }

    private static IEnumerable<string> GetPropertyCreationStatements(TypeContext<ClassDeclarationSyntax> typeContext,
        string nodeName)
    {
        var semanticModel = typeContext.SemanticModel;

        var typeAnalysisResult = typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(e => e.IsFullyPublicProperty())
            .Select(property => new OpcPropertyTypeInfoFactory(property, semanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported);


        foreach (var propertyInfo in typeAnalysisResult.Where(e => e.TypeIsSupported).Select(e => e.PropertyInfo))
        {
            var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
            yield return $"""
                          {nameof(PropertyState)} {propertyName} = {nodeName}.AddProperty<{propertyInfo.Type}>("{propertyName}", {propertyInfo.OpcUaTypeId}, {propertyInfo.ValueRank});
                                     {propertyName}.NodeId = new {nameof(NodeId)}(Guid.NewGuid(), applicationNamespaceIndex);
                          """;
        }
    }
}