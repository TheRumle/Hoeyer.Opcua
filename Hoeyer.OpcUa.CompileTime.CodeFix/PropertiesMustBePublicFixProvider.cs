using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.CompileTime.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.CodeFixers;

/// <summary>
///     Provides fixes for HOEYERUA0001 - OpcUa entities' properties must be fully public./>
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertiesMustBePublicFixProvider))]
[Shared]
public sealed class PropertiesMustBePublicFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Rules.MustHavePublicSetter.Id);

    public override FixAllProvider? GetFixAllProvider()
    {
        return null;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnosticNode = root?.FindNode(diagnosticSpan);
        if (diagnosticNode is not PropertyDeclarationSyntax declaration)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                "Make property publicly available",
                c => MakeFullyPublic(context.Document, declaration, root!),
                Resources.HOEYERUA0001Title.ToString()),
            diagnostic);
    }

    private static Task<Solution> MakeFullyPublic(Document document,
        PropertyDeclarationSyntax property, SyntaxNode root)
    {
        var newProperty = CreatePublicVersionOf(property);
        var newRoot = root.ReplaceNode(property, newProperty);
        var refactoredDocument = document.WithSyntaxRoot(newRoot);
        return Task.FromResult(refactoredDocument.Project.Solution);
    }

    private static PropertyDeclarationSyntax CreatePublicVersionOf(PropertyDeclarationSyntax property)
    {
        var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

        // Create the accessor list with both getter and setter
        var accessorList = SyntaxFactory.AccessorList(
            SyntaxFactory.List([
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
            ]));

        return property
            .WithModifiers(SyntaxFactory.TokenList(publicModifier))
            .WithAccessorList(accessorList);
    }
}