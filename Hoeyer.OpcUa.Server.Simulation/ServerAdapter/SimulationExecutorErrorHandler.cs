using System;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal sealed class SimulationExecutorErrorHandler : ISimulationExecutorErrorHandler
{
    public ServiceResult HandleError(Exception exception)
    {
        return exception switch
        {
            SimulationFailureException simulationFailureException => ServiceResult.Create(simulationFailureException,
                StatusCodes.BadInternalError, "The simulation failed during execution"),
            AggregateException aggregateException => WrapInSimulationFailureException(aggregateException,
                "Multiple exceptions were thrown during the execution of the simulation"),
            ArgumentNullException argumentNullException => WrapInSimulationFailureException(argumentNullException,
                $"The simulation was passed null arguments. Did you forget to provide any args when calling the method?"),
            ArgumentOutOfRangeException ex => WrapInSimulationFailureException(ex,
                $"The simulation threw an {nameof(ArgumentOutOfRangeException)}. Did you call the method with the correct number of arguments?"),
            ArgumentException => ServiceResult.Create(new SimulationFailureException(exception.Message),
                StatusCodes.BadInternalError,
                $"The simulation threw an {nameof(ArgumentOutOfRangeException)}. Did  you call the method with the correct argument types?"),
            var _ => ServiceResult.Create(new SimulationFailureException(exception.Message),
                StatusCodes.BadInternalError,
                null)
        };
    }

    private static ServiceResult WrapInSimulationFailureException(Exception ex, string message = "") =>
        ServiceResult.Create(new SimulationFailureException(ex.Message), StatusCodes.BadInternalError, message);
}