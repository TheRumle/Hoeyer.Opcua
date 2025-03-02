using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.CompileTime.Analysis.Extensions;

public static class PropertyDeclarationSyntaxExtensions
{
    private const int PUBLIC_LENGTH = 6;

    public static bool IsPublic(this PropertyDeclarationSyntax property)
    {
        return property.Modifiers.Any(SyntaxKind.PublicKeyword);
    }

    public static bool IsFullyPublicProperty(this PropertyDeclarationSyntax property)
    {
        var modifierText = property.Modifiers.ToString().AsSpan();
        // The only keyword of length 6 is "public" and no other modifiers must restrict the property.
        // Therefore, any modifier text of length != 6 will restrict access
        // Thus it will not be 'fully public'
        var isPublicProperty = modifierText.Length == PUBLIC_LENGTH;
        return isPublicProperty && property.HasPublicOnlyAccessModifier();
    }


    private static bool HasPublicOnlyAccessModifier(this PropertyDeclarationSyntax property)
    {
        // we know that the property is public
        // if no modifier exists, then there is no public setter
        var accessor = property.Setter();
        if (accessor is null) return false;

        //a public property cannot have the form 'public T t {get; public set;} and any other modifier will indicate that no public setter exists'
        //Therefore, only public T t {get; set;} is validm so ther eshould be no modifier
        return accessor.Modifiers.Count == 0;
    }

    private static AccessorDeclarationSyntax? Setter(this PropertyDeclarationSyntax property)
    {
        return property.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
    }
}