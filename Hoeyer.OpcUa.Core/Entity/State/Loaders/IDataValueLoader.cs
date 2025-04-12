using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Loaders;

public interface IDataValueLoader<in T>
{
    public DataValue Parse(T value);
}