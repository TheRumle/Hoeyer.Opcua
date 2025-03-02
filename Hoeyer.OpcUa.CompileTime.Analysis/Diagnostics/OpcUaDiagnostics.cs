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

    public static readonly DiagnosticDescriptor MustNotHaveNullablePropertyDescriptor = new(
        "HOEYERUA0002",
        "Properties must not be nullable",
        "OpcUa entities' properties can never be null",
        "Design",
        DiagnosticSeverity.Error,
        true,
        "OpcUa entity properties must not be annotated as nullable.");

    public static Diagnostic MustHavePublicSetter(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustHavePublicSetterDescriptor, property.GetLocation(), property.Identifier);
    }

    public static Diagnostic MustNotHaveNullableProperty(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(MustNotHaveNullablePropertyDescriptor, property.GetLocation(), property.Identifier);
    }


    public static DiagnosticDescriptor UnsupportedOpdUaTypeDescriptor(PropertyDeclarationSyntax syntax)
    {
        return new DiagnosticDescriptor(
            "HOEYERUA0002",
            "Unsupported property type",
            $"{syntax.Identifier.ToFullString()} has type '{syntax.Type.ToFullString()}' which is not supported yet",
            "Design",
            DiagnosticSeverity.Error,
            true,
            $"The property '{syntax.Identifier.ToFullString()}' has type '{syntax.Type.ToFullString()}' which is not supported by OpcUa",
            "https://reference.opcfoundation.org/Core/Part6/v104/docs/5.1.2");
    }

    public static Diagnostic UnsupportedOpcUaType(PropertyDeclarationSyntax property)
    {
        return Diagnostic.Create(UnsupportedOpdUaTypeDescriptor(property), property.GetLocation(),
            property.Type.ToFullString(), property.Identifier);
    }
}