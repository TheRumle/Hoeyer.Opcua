using System.Linq;
using Hoeyer.OpcUa.Core.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.SourceGeneration.Generation;

public static class SemanticExtensions
{
    public static string GetBrowseNameFromAttribute(this MemberDeclarationSyntax memberSyntax,
        SemanticModel semanticModel, string fallBack)
    {
        var symbol = semanticModel.GetDeclaredSymbol(memberSyntax);
        var firstArg = symbol?
            .GetAttributes()
            .FirstOrDefault(IsOpcEntityAttributeSymbol)
            ?.ConstructorArguments.First();

        if (firstArg is not { Value: string browseNameValue })
        {
            return fallBack;
        }

        return browseNameValue;
    }

    private static bool IsOpcEntityAttributeSymbol(AttributeData x) =>
        WellKnown.FullyQualifiedAttribute
            .BrowseNameAttribute
            .WithGlobalPrefix
            .Equals(x.AttributeClass?.ToDisplayString(SymbolDisplayFormats.FullyQualifiedNonGenericWithGlobalPrefix));
}