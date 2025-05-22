using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Client.SourceGeneration;

internal class FullyQualifyTypeNamesRewriter(SemanticModel semanticModel) : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node) =>
        RewriteType(node) ?? base.VisitIdentifierName(node)!;

    public override SyntaxNode VisitGenericName(GenericNameSyntax node)
    {
        NameSyntax? rewritten = RewriteType(node);
        if (rewritten != null)
            return rewritten;

        TypeSyntax[] newTypeArguments = node.TypeArgumentList.Arguments
            .Select(arg => (TypeSyntax)Visit(arg))
            .ToArray();

        return node.WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(newTypeArguments)));
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        BaseListSyntax? baseList = node.BaseList;

        if (baseList != null)
        {
            List<BaseTypeSyntax> newBaseTypes = baseList.Types
                .Select(bt =>
                {
                    var newType = (TypeSyntax)Visit(bt.Type);
                    return bt.WithType(newType);
                })
                .ToList();

            baseList = baseList.WithTypes(SyntaxFactory.SeparatedList(newBaseTypes));
        }

        return node.WithBaseList(baseList);
    }

    public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node) =>
        RewriteType(node) ?? base.VisitQualifiedName(node)!;

    public override SyntaxNode VisitNullableType(NullableTypeSyntax node)
    {
        var newElementType = (TypeSyntax)Visit(node.ElementType);
        return node.WithElementType(newElementType);
    }

    public override SyntaxNode VisitArrayType(ArrayTypeSyntax node)
    {
        var newElementType = (TypeSyntax)Visit(node.ElementType);
        return node.WithElementType(newElementType);
    }

    public override SyntaxNode VisitPointerType(PointerTypeSyntax node)
    {
        var newElementType = (TypeSyntax)Visit(node.ElementType);
        return node.WithElementType(newElementType);
    }

    private NameSyntax? RewriteType(TypeSyntax node)
    {
        ITypeSymbol? symbol = semanticModel.GetTypeInfo(node).Type;
        if (symbol != null)
        {
            var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            NameSyntax newName = SyntaxFactory.ParseName(fullyQualifiedName);
            return newName.WithTriviaFrom(node);
        }

        return null;
    }
}