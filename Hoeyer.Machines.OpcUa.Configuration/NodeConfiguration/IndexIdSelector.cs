using System;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;



public class IndexIdSelector
{
    private IndexIdSelector() { }
    private OpcUaNodeSetupContext _context;
    internal IndexIdSelector(OpcUaNodeSetupContext context)
    {
        _context = context;
    }
    public void AndIndexLoadedFrom(Func<NameValueCollection, string> configurationSelector)
    {
        _context.Index = configurationSelector;
    }

}