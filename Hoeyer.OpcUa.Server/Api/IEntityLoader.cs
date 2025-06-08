using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityLoader<T>
{
    public ValueTask<T> LoadCurrentState();
}