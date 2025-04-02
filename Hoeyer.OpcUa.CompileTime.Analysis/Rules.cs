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
        "HOEYERUA0003",
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
    
    public static readonly DiagnosticDescriptor MemberMustBeOpcSupportedDescriptor = new(
        "HOEYERUA0004",
        "Members must be translatable to OpcUa construct",
        "Members must be translatable to OpcUa construct",
        DesignCategory,
        DiagnosticSeverity.Error,
        true,
        "OpcUa Entity classes cannot have members that cannot be translated to OpcUa.");
    
    public static readonly DiagnosticDescriptor MemberMustBeOpcSupported = CreateErrorDescriptor("HOEYERUA0004",
        DesignCategory,
        "The member '{0}' is not supported for OpcUa entities. It must be an empty constructor, property or delegate type.");

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