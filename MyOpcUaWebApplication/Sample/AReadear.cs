using Hoeyer.Machines.OpcUa.Configuration;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;
using Hoeyer.Machines.OpcUa.Configuration.Entities.Property;
using Hoeyer.Machines.OpcUa.Proxy;
using Hoeyer.Machines.Proxy;

namespace MyOpcUaWebApplication.Sample;




public class AReadear(
    IEntityInstanceFactory<Gantry> factory,
    IEntityPropertyAssigner<Gantry, PossiblePropertyMatch> assigner,
    EntityOpcUaMapping<Gantry> settings) 
    : IOpcUaNodeStateReader<Gantry>
{
    
    private readonly List<PropertyConfiguration> _nodes = settings
        .EntityConfiguration
        .PropertyConfigurations
        .ToList();

    /// <inheritdoc />
    public async Task<Gantry> ReadOpcUaEntityAsync(Opc.Ua.Client.Session session)
    {
        try
        {
            
            var readResults = _nodes.Select(async propertyConfiguration => (
                propertyConfiguration,
                dataValue: await session.ReadValueAsync(propertyConfiguration.GetNodeId()))
            );

            var results = await Task.WhenAll(readResults);
            var assignmentResult = assigner.TryAssignToEntity(factory.CreateEmpty, new PossiblePropertyMatch(results));
            return settings.Entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
      
    }


}