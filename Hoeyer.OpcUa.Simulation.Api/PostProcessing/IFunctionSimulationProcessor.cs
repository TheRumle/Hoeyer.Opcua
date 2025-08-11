using Hoeyer.Common.Messaging.Api;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

public interface IFunctionSimulationProcessor<TAgent, TArgs, in TReturn>
    : IStateChangeSimulationProcessor<TAgent>
{
    IMessageConsumer<TReturn> ReturnValueConsumer { get; }
}