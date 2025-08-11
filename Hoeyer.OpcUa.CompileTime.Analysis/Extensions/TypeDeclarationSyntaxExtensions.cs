using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.CodeDomain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class TypeDeclarationSyntaxExtensions
{
    public static bool IsAnnotatedAsOpcUaAgent(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcAgentAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static bool IsAnnotatedAsOpcUaAgent(this ISymbol? symbol)
    {
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcAgentAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }


    public static bool IsAnnotatedAsOpcUaAgentBehaviour(this TypeDeclarationSyntax typeSyntax,
        SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(typeSyntax);
        var isAnnotated = symbol?
            .GetAttributes()
            .Any(IsOpcAgentBehaviourAttributeSymbol);

        return isAnnotated != null && isAnnotated.Value;
    }

    public static AttributeData? GetOpcUaAgentBehaviourAttribute(this TypeDeclarationSyntax typeSyntax,
        SemanticModel semanticModel) =>
        semanticModel
            .GetDeclaredSymbol(typeSyntax)
            ?.GetAttributes()
            .FirstOrDefault(IsOpcAgentBehaviourAttributeSymbol);


    private static bool IsOpcAgentBehaviourAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .AgentBehaviourAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());

    private static bool IsOpcAgentAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .AgentAttribute
            .WithGlobalPrefix.Equals(x.AttributeClass?.GloballyQualifiedNonGeneric());
}