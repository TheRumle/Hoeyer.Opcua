using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

public static class Rules
{
    private const string DesignCategory = "Design";

    public static readonly DiagnosticDescriptor MustHavePublicSetter = CreateErrorDescriptor("HOEYERUA0001",
        DesignCategory, "Entities property must have a public setter.");


    public static readonly DiagnosticDescriptor MustBeSupportedOpcUaType = CreateErrorDescriptor("HOEYERUA0002",
        DesignCategory,
        "The type '{0}' is not supported for OpcUa entities. It must be either a native OpcUa type or an IList of such type with a default constructor.");


    public static readonly DiagnosticDescriptor MustNotBeNullablePropertyDescriptor = new(
        "HOEYERUA0002",
        "Properties must not be nullable",
        "OpcUa entities' properties can never be null",
        DesignCategory,
        DiagnosticSeverity.Error,
        true,
        "OpcUa entity properties must not be annotated as nullable.");

    public static Diagnostic MustNotBeNullableProperty(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustNotBeNullablePropertyDescriptor, property.GetLocation(), property.Identifier);
    }

    private static DiagnosticDescriptor CreateDescriptor(
        string diagnosticId,
        string category,
        DiagnosticSeverity severity,
        string message)
    {
        return new DiagnosticDescriptor(
            diagnosticId,
            message,
            message,
            category,
            severity,
            true,
            message);
    }

    private static DiagnosticDescriptor CreateErrorDescriptor(
        string diagnosticId,
        string category,
        string message)
    {
        return CreateDescriptor(diagnosticId, category, DiagnosticSeverity.Error, message);
    }
}