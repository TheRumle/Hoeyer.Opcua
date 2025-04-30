using System.Collections.Concurrent;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Browsing.Reading;
using Hoeyer.OpcUa.Client.Api.Reading;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public sealed class DepthFirstStrategy(INodeBrowser browser, INodeReader reader) : ConcurrentTreeTraversalStrategy(
    browser, reader,
    () => new ConcurrentStack<ReferenceWithId>());