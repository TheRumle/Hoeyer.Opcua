using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

internal interface ISimulationExecutorErrorHandler
{
    ServiceResult HandleError(Exception exception);
}