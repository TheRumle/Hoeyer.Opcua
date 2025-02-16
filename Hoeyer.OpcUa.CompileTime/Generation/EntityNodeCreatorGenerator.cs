using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Extensions;
using Hoeyer.OpcUa.CompileTime.Generation.IncrementalProvider;
using Hoeyer.OpcUa.CompileTime.OpcUaTypes;
using Hoeyer.OpcUa.Entity;
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
        var propertyCreationStatements = GetPropertyCreationStatements(typeContext, nodeStateReference).ToList();
        var propertyAssignments = string.Join("\n", propertyCreationStatements.Select(e=>e.CreationStatement));

        var propertyNames = string.Join(", ", propertyCreationStatements.Select(e => "\t\t\t\t"+e.PropertyName));
        var resultReturn = $"return new {nameof(EntityNode)}(root, {nodeStateReference}, new List<{nameof(PropertyState)}>()\n\t\t\t{{\n{propertyNames}\n\t\t\t}});";
        
        return $$"""
                 using System;
                 using System.Linq;
                 using Hoeyer.OpcUa.Entity;
                 using Opc.Ua;

                 namespace Hoeyer.OpcUa.Server.Entity
                 {
                     internal sealed class {{className}} : {{nameof(IEntityNodeCreator)}}
                     {
                        public {{className}}(){}
                        public string EntityName { get; } = "{{entityName}}";
                        public {{nameof(EntityNode)}} CreateEntityOpcUaNode({{nameof(FolderState)}} root, ushort applicationNamespaceIndex)
                        {
                            root.AccessRestrictions = AccessRestrictionType.None;
                        w
                            {{nameof(BaseObjectState)}} {{nodeStateReference}} = new {{nameof(BaseObjectState)}}(root)
                            {
                                BrowseName =  new {{nameof(QualifiedName)}}("{{entityName}}", applicationNamespaceIndex),
                                NodeId = new {{nameof(NodeId)}}({{nameof(Guid)}}.{{nameof(Guid.NewGuid)}}(), applicationNamespaceIndex),
                                DisplayName = "{{entityName}}",
                            };
                            {{nodeStateReference}}.AccessRestrictions = AccessRestrictionType.None;
                        
                            root.AddChild({{nodeStateReference}});
                            
                            //Assign properties
                            {{propertyAssignments}}
                            
                            {{resultReturn}}
                        }
                     }
                 }
                 """;
    }

    private static IEnumerable<PropertyCreation> GetPropertyCreationStatements(TypeContext<ClassDeclarationSyntax> typeContext, string nodeName)
    {
        var t = typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(e => e.IsFullyPublicProperty())
            .Select(property => new OpcPropertyTypeInfoFactory(property, typeContext.SemanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported)
            .Select(e=> e.PropertyInfo);
        
        
        foreach (var propertyInfo in t)
        {
            var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
            yield return new PropertyCreation(propertyName, $"""
                          {nameof(PropertyState)} {propertyName} = {nodeName}.AddProperty<{propertyInfo.Type}>("{propertyName}", {propertyInfo.OpcUaTypeId}, {propertyInfo.ValueRank});
                                     {propertyName}.NodeId = new {nameof(NodeId)}(Guid.NewGuid(), applicationNamespaceIndex);
                                     {propertyName}.AccessLevel = AccessLevels.CurrentReadOrWrite;
                          """);
        }
    }


    private record struct PropertyCreation(string PropertyName, string CreationStatement)
    {
        public string CreationStatement { get; set; } = CreationStatement;
        public string PropertyName { get; set; } = PropertyName;
    }
}