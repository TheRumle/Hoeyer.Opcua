using Hoeyer.Machines.OpcUa.Configuration.Entity;
using Hoeyer.Machines.OpcUa.Configuration.Entity.Property;
using Microsoft.Extensions.Options;
using MyOpcUaWebApplication.Options;
using Opc.Ua;

namespace MyOpcUaWebApplication.Sample;



public class GantryConfiguration(IOptions<GantryOptions> options, IOptions<OpcUaRootConfigOptions> rootOptions ) : IOpcUaNodeConfiguration<Gantry>
{
    private readonly GantryOptions _options = options.Value;
    private readonly OpcUaRootConfigOptions _rootOptions = rootOptions.Value;
    /// <inheritdoc />
    public void Configure(IOpcUaEntityConfigurationBuilder<Gantry> gantryConfiguration)
    {
        var gantrySetup = gantryConfiguration.HasRootNodeIdentity(new RootIdentity(_options.Id, _rootOptions.NamespaceIndex));
        
        gantrySetup.Property(e=>e.Name)
                .HasBrowsableName(new NodeIdConfiguration("mainGantry"))
                .AndIsOfType(BuiltInType.String);
    
        gantrySetup
            .Property(e=>e.Speed)
            .HasBrowsableName(new NodeIdConfiguration("machineSpeedMs"))
            .AndIsOfType(BuiltInType.Float);
        
        gantrySetup.Property(e=>e.Id).IsNotMapped();
    }
}