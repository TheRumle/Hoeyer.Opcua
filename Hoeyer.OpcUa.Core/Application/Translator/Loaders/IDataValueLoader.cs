using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator.Loaders;

public interface IDataValueLoader<in T>
{
    public DataValue Parse(T value);
}