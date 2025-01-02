using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Server.Generation.SyntaxExtensions;

public static class PropertyDeclarationSyntaxExtensions
{
    public static bool IsPublic(this PropertyDeclarationSyntax property) => property.Modifiers.Any(SyntaxKind.PublicKeyword);

    public static bool HasPublicSetter(this PropertyDeclarationSyntax property)
    {

        // Check for a public setter. Several scenarios to consider:
        // 1. Explicit public setter,
        // 2. Implicit setter inherited from property: public int MyProperty { ... }
        // 3. Initializer only: public int MyProperty { get; } = 5; (This does not have a setter)
        // 4. Expression body: public int MyProperty => _field; (This does not have a setter)

        var setter = property.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
        if (setter == null)
        {
            return false; // No setter
        }

        //Check if the set accessor has the public modifier. If not present it inherits the visibility of the property itself.
        if (!setter.Modifiers.Any(SyntaxKind.PublicKeyword) && !property.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
            return false; // Setter is not public
        }
        return true;
    }
}