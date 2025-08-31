using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten."),
    SuppressMessage("Maintainability", "S4144",
        Justification =
            "This is a test class and method consuming code will not accidentally call wrong method due to similar signatures.")
]

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;