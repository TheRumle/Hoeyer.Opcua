using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation.IncrementalProvider;

public sealed record TypeContext(SemanticModel SemanticModel, TypeDeclarationSyntax Node)
{
    private readonly IEqualityComparer<UsingDirectiveSyntax>
        UsingDirectiveComparer = new UsingDirectiveSyntaxComparer();

    private IEnumerable<UsingDirectiveSyntax>? _usingDirectives;


    public SemanticModel SemanticModel { get; } = SemanticModel;
    public TypeDeclarationSyntax Node { get; } = Node;
    private INamespaceSymbol NameSpace => SemanticModel.GetDeclaredSymbol(Node)!.ContainingNamespace;

    /// <summary>
    ///     Gets the using statements necessary to compile the Agent.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<IEnumerable<UsingDirectiveSyntax>> GetImports(CancellationToken cancellationToken)
    {
        if (_usingDirectives != null)
        {
            return _usingDirectives;
        }

        var root = await SemanticModel.SyntaxTree.GetRootAsync(cancellationToken);
        var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
        _usingDirectives = usings.Distinct(UsingDirectiveComparer);
        return _usingDirectives;
    }

    /// <summary>
    ///     Gets the using statements necessary to compile the Agent and a using statement for the Agent itself.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UsingDirectiveSyntax>> GetImportsAndContainingNamespace(
        CancellationToken cancellationToken)
    {
        var usings = await GetImports(cancellationToken);
        var u = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(NameSpace.ToString()));
        return new HashSet<UsingDirectiveSyntax>(usings)
        {
            u
        };
    }

    public async Task<CompilationUnitSyntax> CreateCompilationUnitFor(
        ClassDeclarationSyntax classDeclarationSyntax,
        IEnumerable<UsingDirectiveSyntax>? additionalUsings = null,
        CancellationToken cancellationToken = new())
    {
        var usingDirectives = await GetImportsAndContainingNamespace(cancellationToken);

        IEnumerable<UsingDirectiveSyntax> usingStatements = additionalUsings == null
            ? SyntaxFactory.List(usingDirectives.Union(Locations.Utilities))
            : SyntaxFactory.List(usingDirectives.Union(Locations.Utilities)).Union(additionalUsings);

        IEnumerable<UsingDirectiveSyntax> distincts = usingStatements.Distinct(UsingDirectiveComparer);

        return SyntaxFactory.CompilationUnit()
            .AddUsings(distincts.ToArray())
            .AddMembers(Locations.GeneratedPlacement.AddMembers(classDeclarationSyntax));
    }

    private sealed class UsingDirectiveSyntaxComparer : IEqualityComparer<UsingDirectiveSyntax>
    {
        public bool Equals(UsingDirectiveSyntax? x, UsingDirectiveSyntax? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return x.NormalizeWhitespace().ToFullString() == y.NormalizeWhitespace().ToFullString();
        }

        public int GetHashCode(UsingDirectiveSyntax obj) => obj.NormalizeWhitespace().ToFullString().GetHashCode();
    }
}