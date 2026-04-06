using Hoeyer.OpcUa.Client.Test;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Api.Attributes;

namespace Playground.Application.EndToEndTest.Core;

[InheritsTests]
[ReadonlySimulationFixture]
public sealed class BreadthFirstSearchStrategyTests(SimulationTestSession fixture) : BreadthFirstStrategyTest(fixture);