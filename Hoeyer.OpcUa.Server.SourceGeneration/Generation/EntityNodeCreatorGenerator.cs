using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

[Generator]
public class EntityNodeCreatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeDeclarationsDecoratedWith<ClassDeclarationSyntax>("Hoeyer.OpcUa.Core.OpcUaEntityAttribute")
            .Select((e, c) => new TypeContext<ClassDeclarationSyntax>(e.SemanticModel, e.Node));

        context.RegisterImplementationSourceOutput(
            decoratedRecordsProvider.Collect(),
            static (context, typeContexts) =>
            {
                //            foreach (var typeContext in typeContexts) AddNodeGeneratorSourceCode(typeContext, context);
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
        var propertyAssignments = string.Join("\n\t\t\t", propertyCreationStatements.Select(e=>e.CreationStatement));
        
        var addReferenceStatements = string.Join("\n\t\t\t",propertyCreationStatements.Select(e => $"{nodeStateReference}.AddReference(ReferenceTypes.HasProperty, false, {e.PropertyName}.{nameof(NodeId)});"));

        var propertyNames = string.Join(", ", propertyCreationStatements.Select(e => "\n\t\t\t\t"+e.PropertyName));
        var resultReturn = $"return new {nameof(EntityNode)}({nodeStateReference}, new List<{nameof(PropertyState)}>()\n\t\t\t{{\n{propertyNames}\n\t\t\t}});";
        
        return $$"""
                 using System;
                 using System.Linq;
                 using Hoeyer.OpcUa.Core.Entity;
                 using Opc.Ua;

                 namespace Hoeyer.OpcUa.Core.Server.Entity
                 {
                     internal sealed class {{className}} : {{nameof(IEntityNodeCreator)}}
                     {
                        public {{className}}(){}
                        public string EntityName { get; } = "{{entityName}}";
                        public {{nameof(EntityNode)}} CreateEntityOpcUaNode(ushort applicationNamespaceIndex)
                        {
                            {{nameof(BaseObjectState)}} {{nodeStateReference}} = new {{nameof(BaseObjectState)}}(null)
                            {
                                BrowseName =  new {{nameof(QualifiedName)}}("{{entityName}}", applicationNamespaceIndex),
                                NodeId = new {{nameof(NodeId)}}(EntityName, applicationNamespaceIndex),
                                DisplayName = "{{entityName}}",
                            };
                            {{nodeStateReference}}.AccessRestrictions = AccessRestrictionType.None;
                            
                            //Assign properties
                            {{propertyAssignments}}
                            
                            //Add as property references
                            {{addReferenceStatements}}
                            
                            {{resultReturn}}
                        }
                     }
                 }
                 """;
    }

    private static IEnumerable<PropertyCreation> GetPropertyCreationStatements(TypeContext<ClassDeclarationSyntax> typeContext, string nodeName)
    {
        IEnumerable<OpcUaProperty> t = typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(property => new OpcPropertyTypeInfoFactory(property, typeContext.SemanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported)
            .Select(e=> e.PropertyInfo);
        
        foreach (var propertyInfo in t)
        {
            var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
            yield return new PropertyCreation(propertyName, $"""
                          {nameof(PropertyState)} {propertyName} = {nodeName}.AddProperty<{propertyInfo.CSharpType}>("{propertyName}", {propertyInfo.OpcNativeTypeId}, {propertyInfo.ValueRank});
                                     {propertyName}.NodeId = new {nameof(NodeId)}({nameof(Guid)}.{nameof(Guid.NewGuid)}(), applicationNamespaceIndex);
                                     {propertyName}.AccessLevel = AccessLevels.CurrentReadOrWrite;
                                     {propertyName}.DataType = {propertyInfo.OpcNativeTypeId};
                                     
                          """);
        }
    }


    private record struct PropertyCreation(string PropertyName, string CreationStatement);
}