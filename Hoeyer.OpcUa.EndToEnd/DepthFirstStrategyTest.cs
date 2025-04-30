using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[ApplicationFixtureGenerator<DepthFirstStrategy>]
public sealed class DepthFirstStrategyTest(ApplicationFixture<DepthFirstStrategy> fixture) : NodeTreeTraverserTest<DepthFirstStrategy>(fixture);
