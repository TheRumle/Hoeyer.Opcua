using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Models;

public sealed record RemoteMethodCallerModel
{
    public readonly AttributeSyntax Attribute;
    public readonly FullyQualifiedTypeName InterfaceName;
    public readonly InterfaceDeclarationSyntax InterfaceSyntax;
    public readonly FullyQualifiedTypeName RelatedEntityName;

    public RemoteMethodCallerModel(
        INamedTypeSymbol interfaceSymbol,
        INamedTypeSymbol relatedEntity,
        AttributeSyntax attribute,
        InterfaceDeclarationSyntax InterfaceSyntax)
    {
        InterfaceSymbol = interfaceSymbol;
        Attribute = attribute;
        this.InterfaceSyntax = InterfaceSyntax;
        RelatedEntityName = new FullyQualifiedTypeName(relatedEntity);
        InterfaceName = new FullyQualifiedTypeName(interfaceSymbol);
    }

    public INamedTypeSymbol InterfaceSymbol { get; }
}