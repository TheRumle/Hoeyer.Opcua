using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

internal interface ISimulationExecutorErrorHandler
{
    ServiceResult HandleError(Exception exception);
}