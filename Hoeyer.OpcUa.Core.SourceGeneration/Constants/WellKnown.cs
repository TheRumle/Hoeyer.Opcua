using Hoeyer.OpcUa.Core.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Constants;

internal static class WellKnown
{
    public const string CoreServiceName = "Hoeyer.OpcUa.Core.Application";
    public static FullyQualifiedTypeName CoreApiTypeName(string className) => new("Hoeyer.OpcUa.Core.Api." + className);

    private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);

    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName AgentAttribute = CoreTypeName("OpcUaAgentAttribute");

        public static readonly FullyQualifiedTypeName AgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute");

        public static readonly FullyQualifiedTypeName GenericAgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute<T>");


        public static readonly FullyQualifiedTypeName OpcUaAgentServiceAttribute =
            CoreTypeName("OpcUaAgentServiceAttribute");
    }

    public static class FullyQualifiedInterface
    {
        public static FullyQualifiedTypeName IAgent =>
            CoreApiTypeName("IAgent");

        public static FullyQualifiedTypeName DataTypeTranslator =>
            CoreTypeName("Application.OpcTypeMappers.DataTypeToTypeTranslator");

        public static FullyQualifiedTypeName AgentTranslatorInterfaceOf(string T) =>
            CoreApiTypeName($"IAgentTranslator<{T}>");

        public static FullyQualifiedTypeName AgentTranslatorInterfaceOf() =>
            CoreApiTypeName($"IAgentTranslator<>");
    }
}