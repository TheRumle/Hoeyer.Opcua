using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.SourceGeneration.Constants;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;
using Hoeyer.OpcUa.Server.SourceGeneration.OpcUaTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation;

[Generator]
public class EntityNodeFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classProviders = context
            .GetTypeContextForOpcEntities<TypeDeclarationSyntax>()
            .Select(async (typeContext, cancellationToken) =>
                await CreateNodeFactoryCompilationUnit(typeContext, cancellationToken))
            .Select((e, c) => e.Result);

        context.RegisterImplementationSourceOutput(classProviders.Collect(), (productionContext, compilations) =>
        {
            foreach (var factoryAndContext in compilations) factoryAndContext.AddToContext(productionContext);
        });
    }

    private static async Task<GeneratedClass<T>> CreateNodeFactoryCompilationUnit<T>(TypeContext<T> typeContext,
        CancellationToken cancellationToken) where T : TypeDeclarationSyntax
    {
        var propertyInfos = typeContext.Node.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(property => new OpcPropertyTypeInfoFactory(property, typeContext.SemanticModel).GetTypeInfo())
            .Where(e => e.TypeIsSupported)
            .Select(e => e.PropertyInfo);

        var classString = GetClassString(typeContext.Node.Identifier.ToString().TrimEnd(), propertyInfos);
        var classDcl = (await CSharpSyntaxTree
                .ParseText(classString)
                .GetRootAsync(cancellationToken))
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault()!;

        var compilation = await typeContext.CreateCompilationUnitFor(classDcl, Locations.Utilities,
            cancellationToken);
        return new GeneratedClass<T>(compilation, classDcl, typeContext.Node);
    }

    private static IEnumerable<string> GetPropertyNodeDefinitions(string entityName,
        IEnumerable<OpcUaProperty> properties)
    {
        return properties.Select(prop =>
        {
            var propertyName = prop.Name.TrimEnd();
            var dataType = prop.OpcNativeTypeId.TrimEnd();
            var rank = prop.ValueRank.TrimEnd();
            var cSharpType = prop.CSharpType.TrimEnd();
            return $"""
                    {nameof(PropertyState)} {propertyName} = entity.{nameof(BaseInstanceState.AddProperty)}<{cSharpType}>("{propertyName}", {dataType}, {rank});
                    {propertyName}.{nameof(BaseObjectState.NodeId)} = new {nameof(NodeId)}("{entityName}.{propertyName}", applicationNamespaceIndex);
                    {propertyName}.{nameof(PropertyState.AccessLevel)} = {nameof(AccessLevels)}.{nameof(AccessLevels.CurrentReadOrWrite)};
                    entity.{nameof(BaseInstanceState.AddReference)}({nameof(ReferenceTypes)}.{nameof(ReferenceTypes.HasProperty)}, false, {propertyName}.{nameof(PropertyState.NodeId)});
                    yield return {propertyName};
                    """;
        });
    }

    private static string GetClassString(string entityName, IEnumerable<OpcUaProperty> properties)
    {
        var propertyYieldReturns = string.Join("\n\n", GetPropertyNodeDefinitions(entityName, properties));
        return $$"""
                 public sealed class {{entityName}}EntityNodeFactory : {{nameof(IEntityNodeStructureFactory)}}<{{entityName}}>
                 {
                     public string EntityName { get; } = "{{entityName}}";
    
                     public {{nameof(IEntityNode)}} Create(ushort applicationNamespaceIndex)
                     {
                         var entity = CreateEntityBaseObjectState(applicationNamespaceIndex);
                         var properties = CreateProperties(applicationNamespaceIndex, entity);
                         return CreateEntityNode(entity, properties);
                     }
                     
                     private static BaseObjectState CreateEntityBaseObjectState(ushort applicationNamespaceIndex)
                     {
                         BaseObjectState entity = new BaseObjectState(null)
                         {
                             BrowseName =  new QualifiedName("{{entityName}}", applicationNamespaceIndex),
                             NodeId = new NodeId("{{entityName}}", applicationNamespaceIndex),
                             DisplayName = "{{entityName}}",
                         };
                         entity.AccessRestrictions = AccessRestrictionType.None;
                         return entity;
                     }


                     private static {{nameof(IEntityNode)}} CreateEntityNode({{nameof(BaseObjectState)}} entity, IEnumerable<{{nameof(PropertyState)}}> properties)
                     {
                         return new {{nameof(EntityNode)}}(entity, properties.ToList());
                     }

                     private static IEnumerable<{{nameof(PropertyState)}}> CreateProperties(ushort applicationNamespaceIndex, {{nameof(BaseObjectState)}} entity)
                     {
                       {{propertyYieldReturns}}
                     }
                 }
                 """;
    }
}