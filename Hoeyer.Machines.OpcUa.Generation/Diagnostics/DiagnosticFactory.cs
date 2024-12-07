using Microsoft.CodeAnalysis;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

public static class DiagnosticFactory {
    
    internal static Diagnostic UnavailableResourceDiagnostic(string wantedResource)
    {
        var diagnosisDescription = CreateCannotFindAssemblyResourceDescription(wantedResource);
        return Diagnostic.Create(diagnosisDescription, null, DiagnosticSeverity.Warning);
    }


    private static DiagnosticDescriptor CreateCannotFindAssemblyResourceDescription(string wantedResource) => new(
        id: "HOEYER.OPCUA.GENERATOR.CANNOTFINDRESOURCE", // Unique ID for the diagnostic
        title: "Missing resource from assembly.", // Title of the diagnostic
        messageFormat: "Attempted to find resource {0}.", // Format for the message
        category: "SampleCategory", // Category of diagnostics (e.g., Style, Performance)
        DiagnosticSeverity.Warning, // Severity level (e.g., Error, Warning, Info)
        isEnabledByDefault: true,   // Is enabled by default
        description: $"Cannot find the resource {wantedResource}." // Description of the diagnostic,
    );

}