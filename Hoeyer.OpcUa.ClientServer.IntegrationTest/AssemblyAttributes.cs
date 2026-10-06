using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration;
using TUnit.Core.Executors;

[assembly: ParallelLimiter<ParallelLimit>]
[assembly: TestExecutor<MultiAssertExecutor>]
[assembly:
    SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten."),
    SuppressMessage("Maintainability", "S4144")
]