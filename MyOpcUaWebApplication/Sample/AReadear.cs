using Hoeyer.Machines.OpcUa.Configuration.Entity.Context;
using Hoeyer.Machines.OpcUa.Generation.Machine;
using Hoeyer.Machines.OpcUa.Proxy;

namespace MyOpcUaWebApplication.Sample;

public class AReadear(OpcUaEntityConfiguration settings,INodeParser<Gantry> parser) : IOpcUaNodeStateReader<Gantry>
{
    private GantryObservableMachineProxy _proxy;
    
    /// <inheritdoc />
    public async Task<Gantry> ReadOpcUaNodeAsync(Opc.Ua.Client.Session session)
    {
        IEnumerable<Task<Gantry?>> a = settings
            .PropertyConfigurations
            .Select(async propertyConfiguration => parser.Parse(
                propertyConfiguration,
                await session.NodeCache.FetchNodeAsync(propertyConfiguration.GetNodeId)));

        await Task.WhenAll(a);
        return null;
    }
}