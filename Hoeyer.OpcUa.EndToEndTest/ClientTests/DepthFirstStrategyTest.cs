using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[ApplicationFixtureGenerator<DepthFirstStrategy>]
public sealed class DepthFirstStrategyTest(ApplicationFixture<DepthFirstStrategy> fixture)
    : NodeTreeTraverserTest<DepthFirstStrategy>(fixture);