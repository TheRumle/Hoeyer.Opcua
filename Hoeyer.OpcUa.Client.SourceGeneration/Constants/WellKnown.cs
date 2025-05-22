using Hoeyer.OpcUa.Client.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Constants;

internal static class WellKnown
{
    public static class FullyQualifiedAttribute
    {
        public static readonly FullyQualifiedTypeName EntityAttribute = CoreTypeName("OpcUaEntityAttribute");

        public static readonly FullyQualifiedTypeName EntityBehaviourAttribute =
            CoreTypeName("OpcUaEntityMethodsAttribute");

        public static readonly FullyQualifiedTypeName GenericEntityBehaviourAttribute =
            CoreTypeName("OpcUaEntityMethodsAttribute<T>");


        private static readonly FullyQualifiedTypeName OpcUaEntityServiceAttribute =
            CoreTypeName("OpcUaEntityServiceAttribute");

        private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);
    }

    public static class FullyQualifiedInterface
    {
        private static readonly FullyQualifiedTypeName MethodCallerType =
            new("Hoeyer.OpcUa.Client.Api.Calling.IMethodCaller");
    }
}