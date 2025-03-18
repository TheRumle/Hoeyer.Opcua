using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

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
                foreach (var typeContext in typeContexts) AddNodeGeneratorSourceCode(typeContext, context);
            });
    }

    private static void AddNodeGeneratorSourceCode(TypeContext<ClassDeclarationSyntax> typeContext,
        SourceProductionContext context)
    {
        var entityName = typeContext.Node.Identifier.ToString();
        var className = $"{entityName}OpcUaNodeCreator";
        var sourceCode = CreateSourceCode(typeContext, className);
        context.AddSource(className + ".g.cs", sourceCode);
    }

    private static string CreateSourceCode(TypeContext<ClassDeclarationSyntax> typeContext, string className)
    {
        var entityName = typeContext.Node.Identifier;
        var properties = typeContext.Node.Members.OfType<PropertyDeclarationSyntax>().ToList();

        var propertyInitialisations = CreateEntityPropertyInitialisations(properties, typeContext.SemanticModel);
        
        return $$"""
                 using {{typeContext.GetNamespace}};
                 using System;
                 using System.Linq;
                 using Hoeyer.OpcUa.Entity;
                 using Opc.Ua;

                 namespace Hoeyer.OpcUa.Core.Server.Entity
                 {
                     internal sealed class {{className}} : {{nameof(IEntityNodeCreator)}}<{{entityName}}>
                     {
                        public {{entityName}} {{nameof(IEntityNodeCreator<int>.RepresentedEntity)}} {get;} = new {{entityName}}
                        {
                            {{propertyInitialisations}}
                        };
                     
                        public {{className}}(){}
                        public string EntityName { get; } = "{{entityName}}";
                        public {{nameof(EntityNode)}} CreateEntityOpcUaNode(ushort applicationNamespaceIndex)
                        {
                            var entity = GetBaseObjectOpcUaNode(applicationNamespaceIndex);
                            IEnumerable<{{nameof(PropertyState)}}> properties = CreateProperties(entity, applicationNamespaceIndex);
                            AssignPropertyReferences(entity, properties);
                            
                            return new {{nameof(EntityNode)}}(entity, properties.ToList());
                        }
                     
                        private {{nameof(BaseObjectState)}} GetBaseObjectOpcUaNode(ushort applicationNamespaceIndex)
                        {
                            {{nameof(BaseObjectState)}} entity = new {{nameof(BaseObjectState)}}(null)
                            {
                                BrowseName =  new {{nameof(QualifiedName)}}(EntityName, applicationNamespaceIndex),
                                NodeId = new {{nameof(NodeId)}}(EntityName, applicationNamespaceIndex),
                                DisplayName = EntityName,
                            };
                            entity.AccessRestrictions = {{nameof(AccessRestrictionType)}}.{{nameof(AccessRestrictionType.None)}};
                            return entity;
                        }
                         
                        private IEnumerable<{{nameof(PropertyState)}}> CreateProperties({{nameof(BaseObjectState)}} entity, ushort applicationNamespaceIndex)
                        {
                           {{string.Join("\n\t\t  ", GetPropertyCreationStatements(properties, typeContext.SemanticModel))}}
                        }
                        
                        private void AssignPropertyReferences({{nameof(BaseObjectState)}} entity, IEnumerable<{{nameof(PropertyState)}}> properties)
                        {
                           foreach (var property in properties)
                           {
                               entity.AddReference(ReferenceTypes.HasProperty, false, property.NodeId);;
                           }
                        }
                    }
                 }
                 """;
    }
    

    private static string CreateEntityPropertyInitialisations(List<PropertyDeclarationSyntax> properties, SemanticModel semanticModel)
    {
        AssignmentExpressionSyntax CreateAssignmentExpression(PropertyDeclarationSyntax e)
        {
            var typeSymbol = semanticModel.GetTypeInfo(e.Type).Type!;

            var assignedValue =
                SupportedAssignments.CollectionTypes.GetNewDefaultCollectionForType(typeSymbol) as ExpressionSyntax
                ?? SupportedAssignments.SimpleTypes.GetNewDefaultCollectionForType(typeSymbol) as ExpressionSyntax
                ?? DefaultExpression(ParseTypeName(typeSymbol.ToDisplayString()));

            return AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(e.Identifier),
                assignedValue!
            );
        }
        
        return string.Join(",\n \t\t", properties.Select(CreateAssignmentExpression).Select(e => $"{e.ToFullString()};"));
    }

    private static IEnumerable<string> GetPropertyCreationStatements(List<PropertyDeclarationSyntax> properties, SemanticModel semanticModel)
    {
        return properties
            .Select(property => new OpcPropertyTypeInfoFactory(property, semanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported)
            .Select(e => e.PropertyInfo)
            .Select(propertyInfo =>
            {
                var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
                return
                    $"""
                       {nameof(PropertyState)} {propertyName} = entity.AddProperty<{propertyInfo.CSharpType}>("{propertyName}", {propertyInfo.OpcNativeTypeId}, {propertyInfo.ValueRank});
                                 {propertyName}.NodeId = new {nameof(NodeId)}({nameof(Guid)}.{nameof(Guid.NewGuid)}(), applicationNamespaceIndex);
                                 {propertyName}.AccessLevel = AccessLevels.CurrentReadOrWrite;
                                 {propertyName}.AccessRestrictions = AccessRestrictionType.None;
                                 {propertyName}.DataType = {propertyInfo.OpcNativeTypeId};
                                 yield return {propertyName};
                                
                     """;
            });
    }
}