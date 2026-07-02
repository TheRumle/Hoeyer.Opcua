using Hoeyer.OpcUa.Core.Abstractions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.Abstractions;

public interface IOpcMethodArgumentsAttributeUsageValidator
{
    MethodState ValidateAndGetMethodState<TMethodArgs>(IEntityNode managed);
}