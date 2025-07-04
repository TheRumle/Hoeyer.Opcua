using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Action;

internal class ActionSimulationBuilderFactory<TEntity, TMethodArgs>(IEntityTranslator<TEntity> entityTranslator)
    : IActionSimulationBuilderFactory<TEntity, TMethodArgs>
{
    public IActionSimulationBuilder<TEntity, TMethodArgs> CreateBuilder(IManagedEntityNode node) =>
        new ActionSimulationBuilder<TEntity, TMethodArgs>(node,
            new SimulationStepFactory<TEntity, TMethodArgs>(entityTranslator));
}