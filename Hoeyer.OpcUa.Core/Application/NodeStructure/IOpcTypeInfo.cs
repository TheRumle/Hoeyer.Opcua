using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

internal interface IOpcTypeInfo
{
    public BaseInstanceState InstanceState { get; }
}