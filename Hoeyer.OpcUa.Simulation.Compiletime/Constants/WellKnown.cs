using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;

internal static class WellKnown
{
    private static FullyQualifiedTypeName SimulationApiName(string className) =>
        new("Hoeyer.OpcUa.Simulation.Api" + className);

    private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);

    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName EntityAttribute = CoreTypeName("OpcUaEntityAttribute");

        public static readonly FullyQualifiedTypeName EntityBehaviourAttribute =
            CoreTypeName("OpcUaEntityMethodsAttribute");

        public static readonly FullyQualifiedTypeName GenericEntityBehaviourAttribute =
            CoreTypeName("OpcUaEntityMethodsAttribute<T>");


        public static readonly FullyQualifiedTypeName OpcUaEntityServiceAttribute =
            CoreTypeName("OpcUaEntityServiceAttribute");

        public static readonly FullyQualifiedTypeName OpcMethodArgumentsAttribute =
            SimulationApiName(".OpcMethodArgumentsAttribute");
    }

    public static class FullyQualifiedInterface
    {
        public const string SIMULATION_INTERFACE_NAME = "ISimulation";

        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");

        public static readonly FullyQualifiedTypeName IArgsContainer =
            SimulationApiName(".IArgsContainer");

        public static FullyQualifiedTypeName IActionSimulationConfigurator =>
            SimulationApiName($".Configuration.{SIMULATION_INTERFACE_NAME}`2");

        public static FullyQualifiedTypeName IFunctionSimulationConfigurator =>
            SimulationApiName($".Configuration.{SIMULATION_INTERFACE_NAME}`3");

        public static FullyQualifiedTypeName IObjectArgsToTypedArgs(string attributeClassName) =>
            SimulationServerAdapterApiName($".IEntityMethodArgTranslator<{attributeClassName}>");

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
            SimulationApiName($"IEntityMethodArgTranslator<{attributeClassName}>");

        public static FullyQualifiedTypeName TypeTranslatorMethodCall(string genericArg, string arg) =>
            CoreTypeName($"Application.OpcTypeMappers.OpcToCSharpValueParser.ParseTo<{genericArg}>({arg})");
    }
}