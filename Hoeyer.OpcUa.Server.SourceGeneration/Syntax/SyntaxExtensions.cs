using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Syntax;

public static class SyntaxExtensions
{
    public static SeparatedSyntaxList<TNode> ToSeparatedList<TNode>(this IEnumerable<TNode> syntaxNodes)
        where TNode : SyntaxNode
    {
        return SyntaxFactory.SeparatedList(syntaxNodes);
    }


    public static SeparatedSyntaxList<TNode> ToSingletonList<TNode>(this TNode syntaxNodes) where TNode : SyntaxNode
    {
        return SyntaxFactory.SingletonSeparatedList(syntaxNodes);
    }
}