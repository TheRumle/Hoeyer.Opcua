using System;

namespace Hoeyer.OpcUa.Server.Simulation;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OpcEntityMethodSimulatorAttribute(Type entity, string methodName, SimulationMode mode) : Attribute
{
    internal Type EntityType = entity;
    public string MethodName { get; } = methodName;
    public SimulationMode Mode { get; } = mode;
}