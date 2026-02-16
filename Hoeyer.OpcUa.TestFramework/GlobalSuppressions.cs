using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: SuppressMessage(
    "Design", "S3993",
    Justification = "TUnits' attributeusage must not and cannot be overwritten.")]

[assembly: InternalsVisibleTo("Hoeyer.OpcUa.TestFramework.Test")]