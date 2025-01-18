using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Extensions;

public static class AttributeExtensions
{
    public static string FullyQualifiedName(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(attributeSyntax);
        var symbol = symbolInfo.Symbol as INamedTypeSymbol;
        return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
    }
    
}