using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

internal interface IOpcTypeInfo
{
    public BaseInstanceState InstanceState { get; }
}