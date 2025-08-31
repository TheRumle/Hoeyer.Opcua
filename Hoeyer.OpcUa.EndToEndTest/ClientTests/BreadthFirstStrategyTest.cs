using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures;
using Hoeyer.OpcUa.EndToEndTest.Generators;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[InheritsTests]
[TestSubject(typeof(BreadthFirstStrategy))]
[ApplicationFixtureGenerator<BreadthFirstStrategy>]
public sealed class BreadthFirstStrategyTest(ApplicationFixture<BreadthFirstStrategy> fixture)
    : NodeTreeTraverserTest<BreadthFirstStrategy>(fixture);