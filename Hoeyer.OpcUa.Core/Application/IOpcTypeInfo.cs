using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application;

internal interface IOpcTypeInfo
{
    public BaseInstanceState InstanceState { get; }
}