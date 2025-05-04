using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityLoader<T>
{
    public ValueTask<T> LoadCurrentState();
}