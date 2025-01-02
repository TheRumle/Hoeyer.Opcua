using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.Diagnostics;

public sealed class DiagnosticException(Diagnostic diagnostic) : Exception(diagnostic.Descriptor.Title + "\n" + diagnostic.Descriptor.Description)
{
    public readonly Diagnostic Diagnostic = diagnostic;
}

public static class OpcUaDiagnostics
{
    public static readonly DiagnosticDescriptor MustHavePublicSetterDescriptor = new(
        id: "HOEYERUA0001",
        title: "Non-public property setter",
        messageFormat: "Property '{0}' must have a public setter to be a suitable property for an OpcUa Entity",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning, 
        isEnabledByDefault: true,
        description: "Public properties should have a public setter to allow modification.");

    public static Diagnostic MustHavePublicSetter(PropertyDeclarationSyntax property) =>
        Diagnostic.Create(MustHavePublicSetterDescriptor, property.GetLocation(), property.Identifier);

    
    public static readonly DiagnosticDescriptor UnsupportedOpdUaTypeDescriptor = new(
        id: "HOEYERUA0002", 
        title: "Unsupported property type",
        messageFormat: "The type {0} of property '{1}' is not supported yet",
        category: "Design", 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true,
        description: "Public properties should have a public setter to allow modification. The type of property is unsupported. You can find the types supported by OpcUa in the provided uri.", 
        helpLinkUri: "https://reference.opcfoundation.org/Core/Part6/v104/docs/5.1.2");
    
    public static Diagnostic UnsupportedOpcUaType(PropertyDeclarationSyntax property) =>  Diagnostic.Create(UnsupportedOpdUaTypeDescriptor, property.GetLocation(), property.Type.ToFullString(), property.Identifier);
}

