using Hoeyer.OpcUa.Core.Abstractions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.Api;

public interface IOpcMethodArgumentsAttributeUsageValidator
{
    MethodState ValidateAndGetMethodState<TMethodArgs>(IEntityNode managed);
}