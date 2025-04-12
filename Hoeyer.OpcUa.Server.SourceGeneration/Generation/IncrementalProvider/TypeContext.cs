using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;

public sealed record TypeContext<T>(SemanticModel SemanticModel, T Node)
    where T : TypeDeclarationSyntax
{
    private readonly IEqualityComparer<UsingDirectiveSyntax> UsingDirectiveComparer = new UsingComparer();
    private IEnumerable<UsingDirectiveSyntax>? _usingDirectives;


    public SemanticModel SemanticModel { get; } = SemanticModel;
    public T Node { get; } = Node;
    private INamespaceSymbol NameSpace => SemanticModel.GetDeclaredSymbol(Node)!.ContainingNamespace;

    /// <summary>
    ///     Gets the using statements necessary to compile the Entity.
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
    ///     Gets the using statements necessary to compile the Entity and a using statement for the Entity itself.
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

        var usingStatements = additionalUsings == null
            ? SyntaxFactory.List(usingDirectives.Union(Locations.Utilities))
            : SyntaxFactory.List(usingDirectives.Union(Locations.Utilities)).Union(additionalUsings);

        return SyntaxFactory.CompilationUnit()
            .AddUsings(usingStatements.ToArray())
            .AddMembers(Locations.GeneratedPlacement.AddMembers(classDeclarationSyntax));
    }

    private sealed class UsingComparer : IEqualityComparer<UsingDirectiveSyntax>
    {
        /// <inheritdoc />
        public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
        {
            return x.GetText().Equals(y.GetText());
        }

        /// <inheritdoc />
        public int GetHashCode(UsingDirectiveSyntax obj)
        {
            return obj.GetHashCode();
        }
    }
}