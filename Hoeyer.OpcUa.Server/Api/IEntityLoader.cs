using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api;

public interface IAgentLoader<T>
{
    public ValueTask<T> LoadCurrentState();
}