using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

public static class SimulationRules
{
    private const string DesignCategory = "Design";

    public static readonly DiagnosticDescriptor MustBeFunctionSimulation = CreateErrorDescriptor(
        "HOEYERSIMULATION00001",
        DesignCategory,
        "The simulated method returns a value and must be configured as a function");

    public static readonly DiagnosticDescriptor MustBeActionSimulation = CreateErrorDescriptor(
        "HOEYERSIMULATION00002",
        DesignCategory,
        "The simulated method does not returns a value and must be configured as an action");

    public static readonly DiagnosticDescriptor TArgsMustBeAnnotatedWithOpcEntityMethodArgs = CreateErrorDescriptor(
        "HOEYERSIMULATION00003",
        DesignCategory,
        "TArgs must be a type annotated with OpcEntityMethodArgsAttribute");

    public static readonly DiagnosticDescriptor TEntityMustBeAnEntity = CreateErrorDescriptor(
        "HOEYERSIMULATION00004",
        DesignCategory,
        "TEntity must be a type annotated with OpcUaEntityAttribute");

    public static readonly DiagnosticDescriptor TypeHierarchyMustContainOnlyOneActionSimulator = CreateErrorDescriptor(
        "HOEYERSIMULATION00005",
        DesignCategory,
        "The hierarchy of the type must not contain multiple implementations of " + WellKnown
            .FullyQualifiedInterfaceMethodName.IActionSimulationConfigurator.WithoutGlobalPrefix + ".");


    public static readonly DiagnosticDescriptor TypeHierarchyMustContainOnlyOneFunctionSimulator =
        CreateErrorDescriptor(
            "HOEYERSIMULATION00006",
            DesignCategory,
            "The hierarchy of the type must not contain multiple implementations of " + WellKnown
                .FullyQualifiedInterfaceMethodName.IFuncSimulationConfigurator.WithoutGlobalPrefix + ".");


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