using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.SyntaxExtensions;

public static class PropertyDeclarationSyntaxExtensions
{
    public static bool IsPublic(this PropertyDeclarationSyntax property) => property.Modifiers.Any(SyntaxKind.PublicKeyword);

    public static bool IsFullPublicProperty(this PropertyDeclarationSyntax property)
    {
        // public int MyInt {get; set;} -> true otherwise false
        var isPublicProperty = property.Modifiers.Any(SyntaxKind.PublicKeyword);
        return isPublicProperty && property.AccessorList != null &&
               property.AccessorList.Accessors.All(e => e.HasAccessor(SyntaxKind.PublicKeyword));
    }

    public enum GetterOrSetter
    {
        Setter,
        Getter
    }

    private static bool HasAccessability(this PropertyDeclarationSyntax property, GetterOrSetter getterOrSetter, SyntaxKind kind)
    {
        var accessor = property.Accessor(getterOrSetter);
        return accessor != null && accessor.Modifiers.Any(e => e.IsKind(kind));
    }
    
    private static bool DoesNotHaveAccessibility(this PropertyDeclarationSyntax property, GetterOrSetter getterOrSetter, params SyntaxKind[] kind)
    {
        var accessor = property.Accessor(getterOrSetter);
        return accessor != null && accessor.Modifiers.Any(e => kind.Contains(e.Kind()));
    }

    public static AccessorDeclarationSyntax? Accessor(this PropertyDeclarationSyntax property, GetterOrSetter getterOrSetter)
    {
        var propertyAccessor = getterOrSetter == GetterOrSetter.Getter 
            ? SyntaxKind.GetAccessorDeclaration 
            : SyntaxKind.SetAccessorDeclaration;
        return property.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == propertyAccessor);
    }

    public static bool HasAccessor(this AccessorDeclarationSyntax accessor, SyntaxKind kind)
    {
        return accessor.Modifiers.Any(e => e.IsKind(kind));
    }
}