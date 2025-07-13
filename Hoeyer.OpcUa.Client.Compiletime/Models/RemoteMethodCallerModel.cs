using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Models;

public record struct RemoteMethodCallerModel
{
    public readonly InterfaceDeclarationSyntax GlobalizedMethodCaller;
    public readonly FullyQualifiedTypeName InterfaceName;
    public readonly INamedTypeSymbol InterfaceSymbol;
    public readonly FullyQualifiedTypeName RelatedEntityName;

    public RemoteMethodCallerModel(
        INamedTypeSymbol interfaceSymbol,
        INamedTypeSymbol relatedEntity,
        InterfaceDeclarationSyntax globalizedMethodCaller)
    {
        InterfaceSymbol = interfaceSymbol;
        GlobalizedMethodCaller = globalizedMethodCaller;
        RelatedEntityName = new FullyQualifiedTypeName(relatedEntity);
        InterfaceName = new FullyQualifiedTypeName(interfaceSymbol);
    }
}