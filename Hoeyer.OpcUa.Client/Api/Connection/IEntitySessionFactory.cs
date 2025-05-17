using System.Threading;
using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Connection;

public interface IEntitySessionFactory
{
    Task<ISession> CreateSessionAsync(string sessionId, CancellationToken token = default);
    ISession CreateSession(string sessionId);
}