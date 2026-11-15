using Hoeyer.OpcUa.Simulation.Abstractions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IEntityMethodArgTranslator<T> where T : IArgsContainer
{
    public T? Map(IList<Variant> args);
    public Variant[] Map(T args);
}