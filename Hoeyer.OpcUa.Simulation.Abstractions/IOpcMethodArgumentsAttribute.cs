using System;
using System.Reflection;

namespace Hoeyer.OpcUa.Simulation.Abstractions;

public interface IOpcMethodArgumentsAttribute
{
    MethodInfo Method { get; }
    Type Entity { get; }
    Type Interface { get; }
    string MethodName { get; }
}