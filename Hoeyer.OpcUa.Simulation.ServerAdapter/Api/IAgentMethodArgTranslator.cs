using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IAgentMethodArgTranslator<T> where T : IArgsContainer
{
    public T? Map(IList<object> args);
    public object[] Map(T args);
}