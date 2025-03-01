using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Test.Fixtures.Application;
using Hoeyer.OpcUa.Server.Test.TestData;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.OpcUa;




public sealed record PropertyAttributeFixture(uint AttributeId, PropertyState PropertyState, string EntityName)
{
    /// <inheritdoc />
    public override string ToString() => $"{EntityName}: {PropertyState.BrowseName}({Attributes.GetBrowseName(AttributeId)})";
}

/// <summary>
/// Generates <see cref="PropertyAttributeFixture"/> containing an <see cref="Attributes"/> which must be readable by an OpcUa client. 
/// </summary>
[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class ReadablePropertyAttributesGeneratorAttribute : DataSourceGeneratorAttribute<PropertyAttributeFixture>
{

    private static  IEnumerable<ApplicationServiceCollectionFixture> serviceCollections { get; } =
        GeneratedTypes
            .EntityNodeCreators
            .Select(creator => new ApplicationServiceCollectionFixture(creator));

    private static uint[] attributes = new[]
    {
        Attributes.BrowseName,
        Attributes.NodeId,
        Attributes.NodeClass,
        Attributes.DisplayName,
        Attributes.Description,
        Attributes.Value,
        Attributes.ValueRank,
        Attributes.DataType,
        Attributes.MinimumSamplingInterval
    };
    
    /// <inheritdoc />
    public override IEnumerable<Func<PropertyAttributeFixture>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
    {
        return 
            from entityNode in serviceCollections.Select(e => e.EntityNode)
            from property in entityNode.PropertyStates.Values
            from attribute in attributes 
            select (Func<PropertyAttributeFixture>)(() => CreateFixture(attribute, property, entityNode));
    }

    private static PropertyAttributeFixture CreateFixture(uint attribute, PropertyState property, EntityNode entityNode)
    {
        return new PropertyAttributeFixture(attribute, property, entityNode.Entity.BrowseName.Name);
    }
}