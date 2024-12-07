using Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;
using Hoeyer.Machines.OpcUa.Generated.Configuration;
using Hoeyer.Machines.OpcUa.Generation.Machine;
using Hoeyer.Machines.OpcUa.Proxy;
using Hoeyer.Machines.Proxy;
using Opc.Ua;

namespace MyOpcUaWebApplication.Sample;



public class GantryConfiguration : IOpcUaNodeConfiguration<Gantry>
{
    
    /// <inheritdoc />
    public void Configure(NodeSectionSelectionStep<Gantry> gantryConfiguration)
    {
        var sectionSetup = gantryConfiguration.LookForDataInConfigurationSection("opc:gantry");
        
        sectionSetup.ConfigureProperty(e=>e.Id)
                .WithNamespaceFromSection(section => section["namespace"]!)
                .AndIndexLoadedFrom(section => section["index"]!);
        
            sectionSetup
                .ConfigureProperty(e=>e.Speed)
                .WithNamespaceFromSection(section => section["namespace"]!)
                .AndIndexLoadedFrom(section => section["index"]!);
    }
}

/// <summary>
/// A map from field names to opc server sections.
/// </summary>
public class OpcUaGantryNodeStateReaderMap : Dictionary<string, NodeId>
{
    
}
[OpcNodeConfiguration]
public record Gantry(string Name, int Id, int Speed);

public class AReadear(OpcUaGantryNodeStateReaderMap settings) : IOpcUaNodeStateReader<Gantry>
{
    private GantryObservableMachineProxy _proxy;
    
    /// <inheritdoc />
    public async Task<Gantry> ReadOpcUaNodeAsync(Opc.Ua.Client.Session session)
    {
        Node nameNode = await session.NodeCache.FetchNodeAsync(settings["Name"]);
        Node idNode = await session.NodeCache.FetchNodeAsync(settings[nameof(Gantry.Id)]);
        Node speedNode = await session.NodeCache.FetchNodeAsync(settings[nameof(Gantry.Speed)]);
        
        DataValue speedValue = await session.ReadValueAsync(speedNode.NodeId);
        int speed = (speedValue.Value != null) ? Convert.ToInt32(speedValue.Value) : 0;
        return null!;
    }
}
