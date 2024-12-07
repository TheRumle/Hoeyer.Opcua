using System;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace Hoeyer.Machines.OpcUa.Configuration.NodeConfiguration;

public record OpcUaNodeSetupContext
{
    public NameValueCollection? RootSection { get; internal set; }
    public string SectionName { get; internal set; } = string.Empty;
    public Func<NameValueCollection, string> NameSpaceSelector { get;  internal set; }
    public Func<NameValueCollection, string> Index { get;  internal set; }
}