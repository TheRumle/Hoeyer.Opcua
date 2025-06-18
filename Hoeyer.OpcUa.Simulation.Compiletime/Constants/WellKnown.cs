using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;

internal static class WellKnown
{
    private static FullyQualifiedTypeName ServerSimulationName(string className) =>
        new("Hoeyer.OpcUa.Server.Simulation." + className);

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
            ServerSimulationName("OpcMethodArgumentsAttribute");

        private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);
    }

    public static class FullyQualifiedInterface
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");

        public static FullyQualifiedTypeName IActionSimulationConfigurator =>
            ServerSimulationName($"Api.IActionSimulationConfigurator`2");

        public static FullyQualifiedTypeName IFunctionSimulationConfigurator =>
            ServerSimulationName($"Api.IFunctionSimulationConfigurator`3");

        public static FullyQualifiedTypeName IObjectArgsToTypedArgs(string attributeClassName) =>
            ServerSimulationName($"Api.IEntityMethodArgTranslator<{attributeClassName}>");
    }


    public static class FullyQualifiedInterfaceMethodName
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");

        public static FullyQualifiedTypeName IActionSimulationConfigurator =>
            ServerSimulationName($"Api.IActionSimulationConfigurator.");

        public static FullyQualifiedTypeName IFuncSimulationConfigurator =>
            ServerSimulationName($"Api.IFunctionSimulationConfigurator.");

        public static FullyQualifiedTypeName IObjectArgsToTypedArgs(string attributeClassName) =>
            ServerSimulationName($"Api.IEntityMethodArgTranslator<{attributeClassName}>");
    }
}