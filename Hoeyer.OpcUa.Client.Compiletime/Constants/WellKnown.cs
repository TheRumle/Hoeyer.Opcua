using Hoeyer.OpcUa.Client.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Constants;

internal static class WellKnown
{
    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName AgentAttribute = CoreTypeName("OpcUaAgentAttribute");

        public static readonly FullyQualifiedTypeName AgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute");

        public static readonly FullyQualifiedTypeName GenericAgentBehaviourAttribute =
            CoreTypeName("OpcUaAgentMethodsAttribute<T>");


        public static readonly FullyQualifiedTypeName OpcUaAgentServiceAttribute =
            CoreTypeName("OpcUaAgentServiceAttribute");

        private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);
    }

    public static class FullyQualifiedInterface
    {
        public static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");
    }
}