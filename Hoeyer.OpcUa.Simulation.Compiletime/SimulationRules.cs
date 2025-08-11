using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class SimulationRules
{
    private const string DesignCategory = "Design";

    public static readonly DiagnosticDescriptor MustBeFunctionSimulation = CreateErrorDescriptor(
        "HOEYERSIMULATION0001",
        DesignCategory,
        "The simulated method returns a value and must be configured as a function simulator");

    public static readonly DiagnosticDescriptor MustBeActionSimulation = CreateErrorDescriptor(
        "HOEYERSIMULATION0002",
        DesignCategory,
        "The simulated method does not returns a value and must be configured as an action simulator");

    public static readonly DiagnosticDescriptor TArgsMustBeAnnotatedWithOpcEntityMethodArgs = CreateErrorDescriptor(
        "HOEYERSIMULATION0003",
        DesignCategory,
        "TArgs must be a type annotated with OpcEntityMethodArgsAttribute");

    public static readonly DiagnosticDescriptor TEntityMustBeAnEntity = CreateErrorDescriptor(
        "HOEYERSIMULATION0004",
        DesignCategory,
        "TEntity must be a type annotated with OpcUaEntityAttribute");

    public static readonly DiagnosticDescriptor ReturnTypeMustMatchReturnTypeOfSimulatedMethod = CreateErrorDescriptor(
        "HOEYERSIMULATION0005",
        DesignCategory,
        "The configurator is configured to return '{0}' but the simulated method '{1}.{2}' has return type '{3}'"
    );


    public static readonly DiagnosticDescriptor MethodTargetedByTArgsDoesNotExistOnTheTargetedInterface =
        CreateErrorDescriptor(
            "HOEYERSIMULATION0006",
            DesignCategory,
            "The provided TArgs targets an interface '{0}' and method '{1}', but '{0}' does not have an interface of that name");


    private static DiagnosticDescriptor CreateDescriptor(
        string diagnosticId,
        string category,
        DiagnosticSeverity severity,
        string message) =>
        new(
            diagnosticId,
            message,
            message,
            category,
            severity,
            true,
            message);

    private static DiagnosticDescriptor CreateErrorDescriptor(
        string diagnosticId,
        string category,
        string message) =>
        CreateDescriptor(diagnosticId, category, DiagnosticSeverity.Error, message);
}