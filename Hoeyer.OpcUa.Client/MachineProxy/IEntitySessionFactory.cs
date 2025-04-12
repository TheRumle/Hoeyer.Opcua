using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.MachineProxy;

public interface IEntitySessionFactory
{
    Task<ISession> CreateSessionAsync(string sessionId);
    public ApplicationConfiguration Configuration { get; } 
}