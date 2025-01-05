using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.EntityGeneration.Diagnostics;

public sealed class DiagnosticException(Diagnostic diagnostic) : Exception(diagnostic.Descriptor.Title + "\n" + diagnostic.Descriptor.Description)
{
    public readonly Diagnostic Diagnostic = diagnostic;
}

public static class OpcUaDiagnostics
{
    public static readonly DiagnosticDescriptor MustHavePublicSetterDescriptor = new(
        id: "HOEYERUA0001",
        title: "Non-public property setter",
        messageFormat: "OpcUa entities' properties must have a public setters",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true,
        description: "OpcUa entity properties must have a public setter.");

    public static Diagnostic MustHavePublicSetter(PropertyDeclarationSyntax property) =>
        Diagnostic.Create(MustHavePublicSetterDescriptor, property.GetLocation(), property.Identifier);

    
    public static DiagnosticDescriptor UnsupportedOpdUaTypeDescriptor(PropertyDeclarationSyntax syntax) => new(
        id: "HOEYERUA0002", 
        title: "Unsupported property type",
        messageFormat: $"{syntax.Identifier.ToFullString()} has type '{syntax.Type.ToFullString()}' which is not supported yet",
        category: "Design", 
        defaultSeverity: DiagnosticSeverity.Error, 
        isEnabledByDefault: true,
        description: $"The property '{syntax.Identifier.ToFullString()}' has type '{syntax.Type.ToFullString()}' which is not supported by OpcUa", 
        helpLinkUri: "https://reference.opcfoundation.org/Core/Part6/v104/docs/5.1.2");
    
    public static Diagnostic UnsupportedOpcUaType(PropertyDeclarationSyntax property) =>  Diagnostic.Create(UnsupportedOpdUaTypeDescriptor(property), property.GetLocation(), property.Type.ToFullString(), property.Identifier);
}

