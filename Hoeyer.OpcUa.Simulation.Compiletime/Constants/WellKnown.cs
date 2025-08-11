using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;

internal static class WellKnown
{
    private static FullyQualifiedTypeName SimulationApiName(string className) =>
        new("Hoeyer.OpcUa.Simulation.Api" + className);

    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName AgentAttribute = CoreTypeName("OpcUaAgentAttribute");

        public static readonly FullyQualifiedTypeName AgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute");

        public static readonly FullyQualifiedTypeName GenericAgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute<T>");


        public static readonly FullyQualifiedTypeName OpcUaAgentServiceAttribute =
            CoreTypeName("OpcUaAgentServiceAttribute");

        public static readonly FullyQualifiedTypeName OpcMethodArgumentsAttribute =
            SimulationApiName(".OpcMethodArgumentsAttribute");

        private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);
    }

    public static class FullyQualifiedInterface
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");

        public static readonly FullyQualifiedTypeName IArgsContainer =
            SimulationApiName(".IArgsContainer");

        public static FullyQualifiedTypeName IActionSimulationConfigurator =>
            SimulationApiName($".Configuration.ISimulation`2");

        public static FullyQualifiedTypeName IFunctionSimulationConfigurator =>
            SimulationApiName($".Configuration.ISimulation`3");

        public static FullyQualifiedTypeName IObjectArgsToTypedArgs(string attributeClassName) =>
            SimulationServerAdapterApiName($".IAgentMethodArgTranslator<{attributeClassName}>");

        private static FullyQualifiedTypeName SimulationServerAdapterApiName(string s) =>
            new("Hoeyer.OpcUa.Simulation.ServerAdapter.Api" + s);
    }


    public static class FullyQualifiedInterfaceMethodName
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");

        public static FullyQualifiedTypeName IActionSimulationConfigurator =>
            SimulationApiName($"IActionSimulationConfigurator.");

        public static FullyQualifiedTypeName IFuncSimulationConfigurator =>
            SimulationApiName($"IFunctionSimulationConfigurator.");

        public static FullyQualifiedTypeName IObjectArgsToTypedArgs(string attributeClassName) =>
            SimulationApiName($"IAgentMethodArgTranslator<{attributeClassName}>");
    }
}