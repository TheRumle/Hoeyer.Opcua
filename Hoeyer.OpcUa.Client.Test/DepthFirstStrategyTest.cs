using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Api;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Client.Test;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[NotInParallel(nameof(DepthFirstStrategyTest))]
public abstract class DepthFirstStrategyTest(SimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, fixture.GetService<BreadthFirstStrategy>);