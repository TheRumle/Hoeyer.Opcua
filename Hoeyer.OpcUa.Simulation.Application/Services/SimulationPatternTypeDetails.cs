using System;
using System.Linq;
using System.Reflection;

namespace Hoeyer.OpcUa.Simulation.Services;

public record struct SimulationPatternTypeDetails
{
    public SimulationPatternTypeDetails(Type Implementor,
        Type InstantiatedSimulatorInterface,
        Type MethodArgType,
        MethodInfo MethodBeingSimulated,
        Type Agent)
    {
        this.Implementor = Implementor;
        this.InstantiatedSimulatorInterface = InstantiatedSimulatorInterface;
        this.MethodArgType = MethodArgType;
        this.MethodBeingSimulated = MethodBeingSimulated;
        this.Agent = Agent;
        UnwrappedReturnType = MethodBeingSimulated.ReturnType.GetGenericArguments().FirstOrDefault();
    }

    //The return-type task argument, unwrapped
    public Type? UnwrappedReturnType { get; set; }
    public Type Implementor { get; set; }
    public Type InstantiatedSimulatorInterface { get; set; }
    public Type MethodArgType { get; set; }
    public MethodInfo MethodBeingSimulated { get; set; }
    public Type Agent { get; set; }

    public readonly void Deconstruct(out Type Implementor, out Type InstantiatedSimulatorInterface,
        out Type MethodArgType, out Type? UnwrappedReturnType, out Type Agent)
    {
        Implementor = this.Implementor;
        InstantiatedSimulatorInterface = this.InstantiatedSimulatorInterface;
        MethodArgType = this.MethodArgType;
        UnwrappedReturnType = this.UnwrappedReturnType;
        Agent = this.Agent;
    }
}