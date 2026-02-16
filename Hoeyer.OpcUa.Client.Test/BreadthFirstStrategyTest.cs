using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Api;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Test.Client;

[InheritsTests]
[TestSubject(typeof(BreadthFirstStrategy))]
public abstract class BreadthFirstStrategyTest(ISimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, nameof(BreadthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);