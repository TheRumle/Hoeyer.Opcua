using System;
using System.Linq.Expressions;

namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;

public class NodeConfigurationPropertyBuild<TNodeType>
{
    private OpcUaNodeSetupContext _context;
    internal NodeConfigurationPropertyBuild(OpcUaNodeSetupContext _context)
    {
        this._context = _context;
    }
    public NamespaceIdSelector ConfigureProperty<TProperty>(
        Func<TNodeType, TProperty> expression)
    {
        return new NamespaceIdSelector(_context);
    }
}