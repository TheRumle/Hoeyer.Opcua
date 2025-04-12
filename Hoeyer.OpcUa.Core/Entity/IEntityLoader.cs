using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Core.Entity;

public interface IEntityLoader<T>
{
    public ValueTask<T> LoadCurrentState();
}