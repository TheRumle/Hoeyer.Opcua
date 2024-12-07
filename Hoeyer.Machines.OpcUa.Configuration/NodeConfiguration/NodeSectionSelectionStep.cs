using System.Configuration;

namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;

public class NodeSectionSelectionStep<TNodeType> : INodeSectionSelectionStep
{
    public OpcUaNodeSetupContext Context { get; } = new();
    public NodeConfigurationPropertyBuild<TNodeType> LookForDataInConfigurationSection(string sectionName)
    {
        Context.RootSection = ConfigurationManager.GetSection("customSection") as System.Collections.Specialized.NameValueCollection;
        Context.SectionName = sectionName;
        return new NodeConfigurationPropertyBuild<TNodeType>(Context);
    }

}