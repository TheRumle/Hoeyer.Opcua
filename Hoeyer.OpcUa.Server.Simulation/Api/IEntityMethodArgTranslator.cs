using System.Collections.Generic;

namespace Hoeyer.OpcUa.Server.Simulation.Api;

public interface IEntityMethodArgTranslator<T>
{
    public T? Map(IList<object> args);
    public object[] Map(T args);
}