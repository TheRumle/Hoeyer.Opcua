using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public interface IDataValueParser<out T>
{
    public T Parse(DataValue dataValue);
}