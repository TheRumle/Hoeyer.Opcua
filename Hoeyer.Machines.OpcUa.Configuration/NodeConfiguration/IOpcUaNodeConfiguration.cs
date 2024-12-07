namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;


public interface IOpcUaNodeConfiguration<T>
{
    public void Configure(NodeSectionSelectionStep<T> gantryConfiguration);
}