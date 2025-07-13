using System;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Simulation.Api.PostProcessing;

internal interface IFunctionSimulationPipeline<in TEntity, in TArgs, in TReturn>
{
    ValueTask OnSimulationBegin(TArgs args);

    ValueTask ProcessStep(TEntity previous, DateTime timeStamp, TEntity reached,
        ActionType actionType);

    ValueTask OnSimulationFinished();

    ValueTask OnValueReturned(TReturn producedValue);
}