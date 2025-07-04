using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Builder;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;

internal class FunctionSimulationBuilderFactory<TEntity, TMethodArgs, TReturn>(
    IEntityTranslator<TEntity> entityTranslator)
    : IFunctionSimulationBuilderFactory<TEntity, TMethodArgs, TReturn>
{
    public IFunctionSimulationBuilder<TEntity, TMethodArgs, TReturn> CreateBuilder(IManagedEntityNode node) =>
        new FunctionSimulationBuilder<TEntity, TMethodArgs, TReturn>(node,
            new SimulationStepFactory<TEntity, TMethodArgs>(entityTranslator));
}