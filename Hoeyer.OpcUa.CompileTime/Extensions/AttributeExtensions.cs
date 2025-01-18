using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Extensions;

public static class AttributeExtensions
{
    /// <summary>
    ///     For an attribute class 'MyAttribute' that is being used as [My] or [MyAttribute] returns "MyAttribute"
    /// </summary>
    /// <param name="attributeSyntax">The attribute syntax of [My], [MyAttribute], or [My.Namespace.MyAttribute]</param>
    /// <param name="semanticModel">A semantic model used to get the fully qualified format of the attribute usage.</param>
    /// <returns></returns>
    public static string AttributeFullName(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        return semanticModel.GetTypeInfo(attributeSyntax)!.Type!.Name;
    }
}