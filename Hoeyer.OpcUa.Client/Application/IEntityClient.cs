using System.Threading.Tasks;
using FluentResults;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application;

public interface IEntityClient<TValue>
{
    public Task<Result<TValue>> ReadOpcUaEntityAsync(Session session);
}