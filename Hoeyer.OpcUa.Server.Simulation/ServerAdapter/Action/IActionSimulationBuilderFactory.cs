using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;

internal interface IActionSimulationBuilderFactory<TEntity, TMethodArgs>
{
    IActionSimulationBuilder<TEntity, TMethodArgs> CreateBuilder(IManagedEntityNode node);
}