using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.SourceGeneration.Generation.IncrementalProvider;

public struct TypeContext<T>(SemanticModel semanticModel, T node)
    where T : TypeDeclarationSyntax
{
    public SemanticModel SemanticModel { get; } = semanticModel;
    public T Node { get; } = node;
    public INamespaceSymbol NameSpace => SemanticModel.GetDeclaredSymbol(Node)!.ContainingNamespace;
    private IEnumerable<UsingDirectiveSyntax>? _usingDirectives = null;
    
    public async Task<IEnumerable<UsingDirectiveSyntax>> GetImports(CancellationToken cancellationToken)
    {
        if (_usingDirectives != null) return _usingDirectives;
        // Get the root of the syntax tree
        var root = await SemanticModel.SyntaxTree.GetRootAsync(cancellationToken);
        var usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
        _usingDirectives = usings;
        return _usingDirectives;
    }
}