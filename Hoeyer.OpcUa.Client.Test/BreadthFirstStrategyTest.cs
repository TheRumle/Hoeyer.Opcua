using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Api;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Client.Test;

[InheritsTests]
[TestSubject(typeof(BreadthFirstStrategy))]
[NotInParallel(nameof(BreadthFirstStrategyTest))]
public abstract class BreadthFirstStrategyTest(ISimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, fixture.GetService<BreadthFirstStrategy>);