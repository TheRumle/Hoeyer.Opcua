using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.OpcUa.Core.CompileTime.Extensions;

public static class MemberDeclarationSyntaxExtensions
{
    public static (string? violator, Location location) GetIdentifierText(this MemberDeclarationSyntax member)
    {
        string? name = member switch
        {
            PropertyDeclarationSyntax field => field.Identifier.Text,
            EventFieldDeclarationSyntax field => string.Join(", ",
                field.Declaration.Variables.Select(e => e.Identifier.Text)),
            FieldDeclarationSyntax field => string.Join(", ",
                field.Declaration.Variables.Select(e => e.Identifier.Text)),
            BaseFieldDeclarationSyntax field => string.Join(", ",
                field.Declaration.Variables.Select(e => e.Identifier.Text)),
            MethodDeclarationSyntax field => field.Identifier.Text,
            ConstructorDeclarationSyntax field => field.Identifier.Text,
            DestructorDeclarationSyntax field => field.Identifier.Text,
            ClassDeclarationSyntax field => field.Identifier.Text,
            RecordDeclarationSyntax field => field.Identifier.Text,
            StructDeclarationSyntax field => field.Identifier.Text,
            TypeDeclarationSyntax field => field.Identifier.Text,
            BaseTypeDeclarationSyntax field => field.Identifier.Text,
            EnumMemberDeclarationSyntax field => field.Identifier.Text,
            _ => null
        };

        return (name, member.GetLocation());
    }
}