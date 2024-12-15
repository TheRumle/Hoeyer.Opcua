using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Builder;
using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Property;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Options;
using Opc.Ua;

namespace MyOpcUaWebApplication.Sample;



public class GantryConfigurator(IOptions<GantryOptions> options, IOptions<OpcUaRootConfigOptions> rootOptions ) : IOpcEntityConfigurator<Gantry>
{
    private readonly GantryOptions _options = options.Value;
    private readonly OpcUaRootConfigOptions _rootOptions = rootOptions.Value;
    /// <inheritdoc />
    public void Configure(IOpcUaEntityConfigurationBuilder<Gantry> gantryConfiguration)
    {
        var gantrySetup = gantryConfiguration
            .HasRootNodeIdentity(new RootIdentity(_options.Id, _rootOptions.NamespaceIndex))
            .WithEmptyConstructor<Gantry>();
        
        gantrySetup.Property(e=>e.Name)
                .HasNodeId(new NodeIdConfiguration("mainGantry"))
                .AndIsOfType(BuiltInType.String);
    
        gantrySetup
            .Property(e=>e.Speed)
            .HasNodeId(new NodeIdConfiguration("machineSpeedMs"))
            .AndIsOfType(BuiltInType.Float);
        
        gantrySetup.Property(e=>e.Id).IsNotMapped();
    }
}