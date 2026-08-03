using System.Diagnostics.CodeAnalysis;
using Playground.Application.EndToEndTest;
using TUnit.Core.Executors;

[assembly: ParallelLimiter<ParallelLimit>]
[assembly: TestExecutor<MultiAssertExecutor>]
[assembly:
    SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten."),
    SuppressMessage("Maintainability", "S4144",
        Justification =
            "This is ISimulationContainer test class and method consuming code will not accidentally call wrong method due to similar signatures.")
]