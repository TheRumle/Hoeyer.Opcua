using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration.Models;

public record struct RemoteMethodCallerModel
{
    public readonly InterfaceDeclarationSyntax GlobalizedMethodCaller;
    public readonly FullyQualifiedTypeName InterfaceName;
    public readonly INamedTypeSymbol InterfaceSymbol;
    public readonly FullyQualifiedTypeName RelatedAgentName;

    public RemoteMethodCallerModel(
        INamedTypeSymbol interfaceSymbol,
        INamedTypeSymbol relatedAgent,
        InterfaceDeclarationSyntax globalizedMethodCaller)
    {
        InterfaceSymbol = interfaceSymbol;
        GlobalizedMethodCaller = globalizedMethodCaller;
        RelatedAgentName = new FullyQualifiedTypeName(relatedAgent);
        InterfaceName = new FullyQualifiedTypeName(interfaceSymbol);
    }
}