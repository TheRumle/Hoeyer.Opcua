using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

internal interface IFunctionSimulationPipeline<in TAgent, in TArgs, in TReturn>
{
    ValueTask OnSimulationBegin(TArgs args);

    ValueTask ProcessStep(TAgent previous, DateTime timeStamp, TAgent reached,
        ActionType actionType);

    ValueTask OnSimulationFinished();

    ValueTask OnValueReturned(TReturn producedValue);
}