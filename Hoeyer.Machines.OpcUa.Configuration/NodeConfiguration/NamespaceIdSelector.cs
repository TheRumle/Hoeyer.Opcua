using System;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;


public class NamespaceIdSelector
{
    private OpcUaNodeSetupContext _context;

    internal NamespaceIdSelector(OpcUaNodeSetupContext context)
    {
        _context = context;
    }
    
    public IndexIdSelector WithNamespaceFromSection(Func<NameValueCollection, string> configurationSelector)
    {
        _context.NameSpaceSelector = configurationSelector;
        return new IndexIdSelector(_context);
    }
}