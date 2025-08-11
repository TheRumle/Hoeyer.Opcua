using System;
using System.Reflection;

namespace Hoeyer.OpcUa.Simulation.Api;

public interface IOpcMethodArgumentsAttribute
{
    MethodInfo Method { get; }
    Type Agent { get; }
    Type Interface { get; }
    string MethodName { get; }
}