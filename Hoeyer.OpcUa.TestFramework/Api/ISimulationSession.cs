using Hoeyer.OpcUa.Test.Simulation;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test.Api;

public interface ISimulationSession : IAsyncDisposable, IAsyncInitializer
{
    SimulationSetup SimulationSetup { get; }
}