using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Simulation.Abstractions.PostProcessing;

public interface IFunctionSimulationProcessor<TEntity, TArgs, in TReturn>
    : IStateChangeSimulationProcessor<TEntity>
{
    IMessageConsumer<TReturn> ReturnValueConsumer { get; }
}