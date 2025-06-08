using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;

internal static class WellKnown
{
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

        private static FullyQualifiedTypeName ServerSimulationName(string className) =>
            new("Hoeyer.OpcUa.Server.Simulation." + className);
    }

    public static class FullyQualifiedInterface
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");
    }
}