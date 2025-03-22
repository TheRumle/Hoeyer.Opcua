using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.SourceGeneration.Constants;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;
using Hoeyer.OpcUa.Server.SourceGeneration.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

[Generator]
public class EntityNodeCreatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedRecordsProvider = context
            .GetTypeContextForOpcEntities<TypeDeclarationSyntax>()
            .Select(async (e, c) => await GenerateClass(e, c))
            .Select((e,c) => e.Result);

        context.RegisterImplementationSourceOutput(decoratedRecordsProvider.Collect(), AddContainerSourceCode);
    }
    
    
    private static void AddContainerSourceCode(SourceProductionContext context, ImmutableArray<GeneratedClass<TypeDeclarationSyntax>> generatedUnits)
    {
        //is not supported and will not be for a while
    }

    private static async Task<GeneratedClass<T>> GenerateClass<T>(TypeContext<T> typeContext, CancellationToken token) where T : TypeDeclarationSyntax
    {
        var entityName = IdentifierName(typeContext.Node.Identifier.ToString());
        var nodeStateReference = IdentifierName(entityName + "Node");
        var className = Identifier($"{entityName}OpcUaNodeCreator");
        
        var nodeCreationStatement = GetNodeCreationMethod(typeContext, nodeStateReference);
        
        var entityNameMethod = ClassConstruction.PropertyWithPublicGetter(SyntaxKind.StringKeyword, nameof(IEntityNodeCreator.EntityName), entityName.ToString());
        
        var generatedClass = typeContext.CreateClassImplementingFromEntityGeneric(className,
                @interface: nameof(IEntityNodeCreator), //uses generic version! 
                members:[entityNameMethod, nodeCreationStatement]);
        
        var unit = await typeContext
            .CreateCompilationUnitFor(
                classDeclarationSyntax: generatedClass,
                additionalUsings: Locations.Utilities.ToArray(),
                cancellationToken: token);

        return new GeneratedClass<T>(unit, generatedClass, typeContext.Node);
    }

    private static MethodDeclarationSyntax GetNodeCreationMethod<T>(TypeContext<T> typeContext,
        IdentifierNameSyntax nodeStateReference) where T : TypeDeclarationSyntax
    {
        var nodeType = ParseTypeName(nameof(BaseObjectState));
        var namespaceIndexName = IdentifierName("applicationNamespaceIndex");
        
        
        var nodeCreationStatement = NodeCreationStatement(nodeStateReference, namespaceIndexName);
        var propertyCreationStatements = PropertyCreationStatements(typeContext, nodeStateReference);
        
        
        var method = ClassConstruction.PublicMethodWithArgs(nameof(IEntityNodeCreator.CreateEntityOpcUaNode), nodeType,
            [(namespaceIndexName.Identifier.ToFullString(), ParseTypeName("ushort"))],
            [nodeCreationStatement, ..propertyCreationStatements]);

        return method;
    }

    private static IReadOnlyList<StatementSyntax> PropertyCreationStatements<T>(TypeContext<T> typeContext, IdentifierNameSyntax nodeName) where T : TypeDeclarationSyntax
    {
        return typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(property => new OpcPropertyTypeInfoFactory(property, typeContext.SemanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported)
            .Select(e => e.PropertyInfo)
            .SelectMany(propertyInfo => GetPropertyInitialisationStatements(nodeName.ToString(), propertyInfo))
            .ToList();
    }

    private static IEnumerable<StatementSyntax> GetPropertyInitialisationStatements(string nodeName, OpcUaProperty propertyInfo)
    {
        var propertyName = char.ToLower(propertyInfo.Name.Trim()[0]) + propertyInfo.Name.Trim().Substring(1);
        var addPropertyCall = $"{nodeName}.{nameof(BaseObjectState.AddProperty)}<{propertyInfo.CSharpType}>(\"{propertyName}\", {propertyInfo.OpcNativeTypeId}, {propertyInfo.ValueRank})";

        List<string> statements = [
            $"{nameof(PropertyState)}{propertyName} = {addPropertyCall};", // addProperty
            $"{propertyName}.NodeId = new {nameof(NodeId)}({nameof(Guid)}.{nameof(Guid.NewGuid)}(), applicationNamespaceIndex);", // createNode
            $"{propertyName}.AccessLevel = AccessLevels.CurrentReadOrWrite;", // assignAccessLevel
            $"{propertyName}.DataType = {propertyInfo.OpcNativeTypeId};", // assignDataType
            $"{nodeName}.AddReference(ReferenceTypes.HasProperty, false, {propertyInfo.Name}.{nameof(NodeId)});" // addParentReference
        ];
        
        return statements.Select(e => ParseStatement(e));
    }


    private static StatementSyntax NodeCreationStatement(IdentifierNameSyntax objectState, IdentifierNameSyntax namespaceIndex)
    {
        
        
        var nullArg = Argument(LiteralExpression(SyntaxKind.NullLiteralExpression));
        
        var browseName = ClassConstruction.AssignToNewObject("BrowseName", nameof(QualifiedName),
            nameof(IEntityNodeCreator.EntityName), namespaceIndex.ToString());

        var nodeId = ClassConstruction.AssignToNewObject("NodeId", nameof(NodeId),
            nameof(IEntityNodeCreator.EntityName), namespaceIndex.ToString());

        var displayName = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            IdentifierName("DisplayName"), IdentifierName(nameof(IEntityNodeCreator.EntityName)));
        
            
            
            
        var newBaseObjectState =  ClassConstruction.ObjectInstantiationWithPropertyAssignments(objectState,
            [nullArg], [browseName, nodeId, displayName]);

        
        
        
        
        var nodeObjectCreation = $$"""
                                   {{newBaseObjectState}}
                                   {{nameof(BaseObjectState)}} {{objectState}} = new {{nameof(BaseObjectState)}}(null)
                                              {
                                                  BrowseName =  new {{nameof(QualifiedName)}}({{nameof(IEntityNodeCreator.EntityName)}}, applicationNamespaceIndex),
                                                  NodeId = new {{nameof(NodeId)}}({{nameof(IEntityNodeCreator.EntityName)}}, applicationNamespaceIndex),
                                                  DisplayName = {{nameof(IEntityNodeCreator.EntityName)}},
                                              };
                                              {{objectState}}.AccessRestrictions = AccessRestrictionType.None;
                                   """;
        return ParseStatement(nodeObjectCreation);
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