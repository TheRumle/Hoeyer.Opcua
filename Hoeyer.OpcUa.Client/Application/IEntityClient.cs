using System.Threading.Tasks;
using FluentResults;
using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application;

public interface IEntityClient
{
    public Task<Result<IEntityNode>> ReadOpcUaEntityAsync(Session session);
}