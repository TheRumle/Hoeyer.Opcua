using System.Reflection;

namespace Hoeyer.OpcUa.Core;

public interface IOpcMethodArgumentsAttribute
{
    MethodInfo Method { get; }
    Type Entity { get; }
    Type Interface { get; }
    string MethodName { get; }
}