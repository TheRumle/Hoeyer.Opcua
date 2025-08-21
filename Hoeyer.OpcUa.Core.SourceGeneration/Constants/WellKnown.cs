using Hoeyer.OpcUa.Core.SourceGeneration.Models;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Constants;

internal static class WellKnown
{
    public const string CoreServiceName = "Hoeyer.OpcUa.Core.Application";
    public static FullyQualifiedTypeName CoreApiTypeName(string className) => new("Hoeyer.OpcUa.Core.Api." + className);

    private static FullyQualifiedTypeName CoreTypeName(string className) => new("Hoeyer.OpcUa.Core." + className);

    public static class FullyQualifiedInterface
    {
        public static FullyQualifiedTypeName IEntityNode =>
            CoreApiTypeName("IEntityNode");

        public static FullyQualifiedTypeName DataTypeTranslator =>
            CoreTypeName("Application.OpcTypeMappers.DataTypeToTypeTranslator");

        public static FullyQualifiedTypeName EntityTranslatorInterfaceOf(string T) =>
            CoreApiTypeName($"IEntityTranslator<{T}>");

        public static FullyQualifiedTypeName EntityTranslatorInterfaceOf() =>
            CoreApiTypeName($"IEntityTranslator<>");
    }
}