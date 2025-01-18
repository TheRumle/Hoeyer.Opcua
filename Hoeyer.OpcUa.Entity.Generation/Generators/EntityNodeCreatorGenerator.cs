using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime;
using Hoeyer.OpcUa.CompileTime.Extensions;
using Hoeyer.OpcUa.CompileTime.OpcUaTypes;
    using Hoeyer.OpcUa.EntityGeneration.IncrementalProvider;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.EntityGeneration.Generators;


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
            foreach (var typeContext in typeContexts)
            {
                AddNodeGeneratorSourceCode(typeContext, context);
            }
        });
    }

    private static void AddNodeGeneratorSourceCode(TypeContext<ClassDeclarationSyntax> typeContext, SourceProductionContext context)
    {
        var entityName = typeContext.Node.Identifier.ToString();
        var nodeName = entityName + "Node";
        var className = $"{entityName}OpcUaNodeCreator";
        var sourceCode = CreateSourceCode(typeContext, context, entityName, nodeName, className);
        context.AddSource(className+".g.cs", sourceCode);
    }

    private static string CreateSourceCode(TypeContext<ClassDeclarationSyntax> typeContext, SourceProductionContext context, string entityName,
        string nodeName, string className)
    {
        return $$"""
using System;
using System.Linq;
using Hoeyer.OpcUa.Nodes;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity
{
    internal sealed class {{className}} : IEntityNodeCreator
    {
       public {{className}}(){}
       public string EntityName { get; } = "{{entityName}}";
       public NodeState CreateEntityOpcUaNode(NodeState root, ushort dynamicNamespaceIndex)
       {
           BaseObjectState {{nodeName}} = new BaseObjectState(root)
           {
               BrowseName =  new QualifiedName("{{entityName}}", dynamicNamespaceIndex),
               NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex),
               DisplayName = "{{entityName}}",
           };
       
           root.AddChild({{nodeName}});
           
           //Assign properties
           {{string.Join("\n", GetPropertyCreationStatements(typeContext, context, nodeName))}}
           
           return {{nodeName}};
       }
    }
}
""";
    }

    private static IEnumerable<string> GetPropertyCreationStatements(TypeContext<ClassDeclarationSyntax> typeContext,, string nodeName)
    {
        var semanticModel = typeContext.SemanticModel;

        var typeAnalysisResult = typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(e => e.IsFullyPublicProperty())
            .Select(property => new OpcPropertyTypeInfoFactory(property, semanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported);

        
        foreach (var propertyInfo in typeAnalysisResult.Where(e=>e.TypeIsSupported).Select(e=>e.PropertyInfo))
        {
            var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
            yield return $"""
PropertyState {propertyName} = {nodeName}.AddProperty< {propertyInfo.Type}>("{propertyName}", {propertyInfo.OpcUaTypeId}, ValueRanks.Scalar);
           {propertyName}.NodeId = new NodeId(Guid.NewGuid(), dynamicNamespaceIndex);
""";
        }
        
    }


}