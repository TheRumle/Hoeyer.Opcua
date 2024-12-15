using System.Threading.Tasks;
using FluentResults;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Proxy;

public interface IOpcUaNodeStateReader<TValue>
{
    public Task<Result<TValue>> ReadOpcUaEntityAsync(Session session);
}