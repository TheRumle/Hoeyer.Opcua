using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MemberMustBeSupported() : ConcurrentAnalyzer([Rules.MemberMustBeOpcSupported])
{
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
        {
            return;
        }
        
        var supportedMemberSyntax = typeSyntax
            .Members
            .Where(member => member switch
            {
                PropertyDeclarationSyntax => false,
                EventDeclarationSyntax => false,
                EventFieldDeclarationSyntax => false,
                EnumDeclarationSyntax=> false,
                DelegateDeclarationSyntax => false,
                _ => true
            })
            .Select(GetIdentifierText);

        foreach (var member in supportedMemberSyntax)
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Rules.MemberMustBeOpcSupportedDescriptor,
                    member.location,
                    messageArgs: member.typeName
                )
            );
    }

    private static (string? typeName, Location location) GetIdentifierText(MemberDeclarationSyntax member)
    {
        string? name = member switch
        {
            PropertyDeclarationSyntax field => field.Identifier.Text,
            EventFieldDeclarationSyntax field => string.Join(", ", field.Declaration.Variables.Select(e => e.Identifier.Text)),
            FieldDeclarationSyntax field => string.Join(", ", field.Declaration.Variables.Select(e => e.Identifier.Text)),
            BaseFieldDeclarationSyntax field => string.Join(", ", field.Declaration.Variables.Select(e => e.Identifier.Text)),
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