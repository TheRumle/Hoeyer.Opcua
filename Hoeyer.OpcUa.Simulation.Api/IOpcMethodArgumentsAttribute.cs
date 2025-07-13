using System;
using System.Reflection;

namespace Hoeyer.OpcUa.Simulation.Api;

public interface IOpcMethodArgumentsAttribute
{
    MethodInfo Method { get; }
    Type Entity { get; }
    Type Interface { get; }
    string MethodName { get; }
}