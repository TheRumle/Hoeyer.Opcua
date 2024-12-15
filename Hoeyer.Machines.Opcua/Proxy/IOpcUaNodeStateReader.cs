using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Proxy;

public interface IOpcUaNodeStateReader<TNode>
{
    public Task<TNode> ReadOpcUaEntityAsync(Session session);
}