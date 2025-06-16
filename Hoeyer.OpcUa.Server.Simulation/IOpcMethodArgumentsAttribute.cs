using System;
using System.Reflection;

namespace Hoeyer.OpcUa.Server.Simulation;

internal interface IOpcMethodArgumentsAttribute
{
    MethodInfo Method { get; }
    Type Entity { get; }
    Type Interface { get; }
    string MethodName { get; }
}