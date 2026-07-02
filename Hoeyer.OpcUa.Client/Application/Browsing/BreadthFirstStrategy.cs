using System.Collections.Concurrent;
using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Browsing.Reading;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public sealed class BreadthFirstStrategy(INodeBrowser browser, INodeReader reader) : ConcurrentTreeTraversalStrategy(
    browser, reader,
    () => new ConcurrentQueue<ReferenceWithId>());