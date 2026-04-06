using Hoeyer.OpcUa.Test.Simulation;

namespace Hoeyer.OpcUa.Test.Api;

public interface ISimulationServiceContext<T> : ISimulationSession
{
    IServiceProvider ServiceProvider { get; }
    List<ISimulationTestContext<T>> GetSimulationSession();
}