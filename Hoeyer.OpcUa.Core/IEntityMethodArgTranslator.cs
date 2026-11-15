using Opc.Ua;

namespace Hoeyer.OpcUa.Core;

public interface IEntityMethodArgTranslator<T> where T : IArgsContainer
{
    public T? Map(IList<Variant> args);
    public Variant[] Map(T args);
}