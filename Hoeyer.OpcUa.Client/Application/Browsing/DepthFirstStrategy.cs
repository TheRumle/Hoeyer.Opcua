using System.Collections.Concurrent;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public sealed class DepthFirstStrategy(INodeBrowser browser, INodeReader reader) : ConcurrentTreeTraversalStrategy(
    browser, reader,
    () => new ConcurrentStack<ReferenceWithId>());