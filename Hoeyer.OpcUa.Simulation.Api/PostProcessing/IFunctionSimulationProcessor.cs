using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

public interface IFunctionSimulationProcessor<TEntity, TArgs, in TReturn>
    : IStateChangeSimulationProcessor<TEntity>
{
    IMessageConsumer<TReturn> ReturnValueConsumer { get; }
}