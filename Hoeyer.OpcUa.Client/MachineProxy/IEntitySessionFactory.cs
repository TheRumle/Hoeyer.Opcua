using System.Threading.Tasks;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.MachineProxy;

public interface IEntitySessionFactory
{
    Task<ISession> CreateSessionAsync(string sessionId);
    ISession CreateSession(string sessionId);
}