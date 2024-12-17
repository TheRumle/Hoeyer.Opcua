using System.Threading.Tasks;
using FluentResults;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Client.Domain;

public interface IOpcUaNodeConnectionHolder<TValue>
{
    public Task<Result<TValue>> ReadOpcUaEntityAsync(Session session);
}