using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter.Function;

internal interface IFunctionSimulationBuilderFactory<TEntity, TMethodArgs, in TReturn>
{
    IFunctionSimulationBuilder<TEntity, TMethodArgs, TReturn> CreateBuilder(IManagedEntityNode node);
}