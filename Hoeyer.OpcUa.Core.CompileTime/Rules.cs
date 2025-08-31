using Hoeyer.OpcUa.Core.CompileTime.CodeDomain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.CompileTime;

public static class Rules
{
    private const string DesignCategory = "Design";

    public static readonly DiagnosticDescriptor MustHavePublicSetter = CreateErrorDescriptor(
        "HOEYERUA0001",
        DesignCategory,
        "Entities property must have a public setter.");


    public static readonly DiagnosticDescriptor MustBeSupportedOpcUaType = CreateErrorDescriptor(
        "HOEYERUA0002",
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


    public static readonly DiagnosticDescriptor OpcUaEntityMemberNotSupported = CreateErrorDescriptor(
        "HOEYERUA0004",
        DesignCategory,
        "The member is not supported for OpcUa entity definitions");

    public static readonly DiagnosticDescriptor OpcUaEntityBehaviourMemberNotSupported = CreateErrorDescriptor(
        "HOEYERUA0005",
        DesignCategory,
        "The member is not supported for OpcUa entity behaviour definitions.");


    public static readonly DiagnosticDescriptor MustBeOpcEntityArgument = CreateErrorDescriptor(
        "HOEYERUA0006",
        DesignCategory,
        $"The argument must be a type annotated with '{WellKnown.FullyQualifiedAttribute.EntityAttribute.WithoutGlobalPrefix}'.");

    public static readonly DiagnosticDescriptor ReturnTypeMustBeTask = CreateErrorDescriptor(
        "HOEYERUA0007",
        DesignCategory,
        "The return of entity methods must be of type Task or Task<T> and T must be either a native OpcUa type or an IList of such type with a default constructor.");

    public static readonly DiagnosticDescriptor MethodNameMustBeUnique = CreateErrorDescriptor(
        "HOEYERUA0008",
        DesignCategory,
        "Entity methods must be uniquely identified by their name.");

    public static Diagnostic MustNotBeNullableProperty(PropertyDeclarationSyntax property) =>
        Diagnostic.Create(MustNotBeNullablePropertyDescriptor, property.GetLocation(), property.Identifier);

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