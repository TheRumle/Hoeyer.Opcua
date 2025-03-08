using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Diagnostics;

public static class OpcUaDiagnostics
{
    public static readonly DiagnosticDescriptor MustHavePublicSetterDescriptor = new(
        "HOEYERUA0001",
        "Non-public property setter",
        "OpcUa entities' properties must have a public setters",
        "Design",
        DiagnosticSeverity.Error,
        true,
        "OpcUa entity properties must have a public setter.");

    public static readonly DiagnosticDescriptor MustNotBeNullablePropertyDescriptor = new(
        "HOEYERUA0002",
        "Properties must not be nullable",
        "OpcUa entities' properties can never be null",
        "Design",
        DiagnosticSeverity.Error,
        true,
        "OpcUa entity properties must not be annotated as nullable.");
    
    public static readonly DiagnosticDescriptor MustBeSupportedTypeDescriptor = new(
        "HOEYERUA0003",
        "Unsupported type",
        "The type of the property is not supported",
        "Design",
        DiagnosticSeverity.Error,
        true,
        "The type of the property is not supported.");

    public static Diagnostic MustHavePublicSetter(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustHavePublicSetterDescriptor, property.GetLocation(), property.Identifier);
    }

    public static Diagnostic MustNotBeNullableProperty(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustNotBeNullablePropertyDescriptor, property.GetLocation(), property.Identifier);
    }
    
    public static Diagnostic MustBeSupportedType(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustBeSupportedTypeDescriptor, property.GetLocation(), property.Identifier);
    }


}