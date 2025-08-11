using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.CompileTime.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityAnalyzer() : ConcurrentAnalyzer([
    Rules.OpcUaEntityMemberNotSupported,
    Rules.MustBeSupportedOpcUaType,
    Rules.MustHavePublicSetter,
    Rules.MustNotBeNullablePropertyDescriptor
])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeSupportedMembers, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeSupportedMembers, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeSupportedMembers(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaEntity(context.SemanticModel))
        {
            return;
        }

        foreach (var diagnostic in FindViolations(typeSyntax, context).AsParallel())
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static IEnumerable<Diagnostic>
        FindViolations(TypeDeclarationSyntax typeSyntax, SyntaxNodeAnalysisContext context)
    {
        var supportedMembers = typeSyntax
            .Members
            .Where(member => member switch
            {
                PropertyDeclarationSyntax => true,
                EnumDeclarationSyntax => true,
                MethodDeclarationSyntax => true,
                TypeDeclarationSyntax => true,
                _ => false
            }).ToList();

        var unsupportedMembers = typeSyntax
            .Members
            .Except(supportedMembers)
            .ToList();

        var errors = unsupportedMembers
            .Select(e => e.GetIdentifierText())
            .Select(e => Diagnostic.Create(
                Rules.OpcUaEntityMemberNotSupported,
                e.location
            ))
            .Concat(GetTypeViolations(context, supportedMembers))
            .Concat(GetAssessabilityViolations(supportedMembers))
            .Concat(NullabilityViolations(supportedMembers))
            .ToList();

        return errors;
    }

    private static IEnumerable<Diagnostic> NullabilityViolations(List<MemberDeclarationSyntax> supportedMembers)
    {
        return supportedMembers.OfType<PropertyDeclarationSyntax>()
            .Where(e => e.Type is NullableTypeSyntax)
            .Select(Rules.MustNotBeNullableProperty);
    }

    private static IEnumerable<Diagnostic> GetAssessabilityViolations(List<MemberDeclarationSyntax> supportedMembers)
    {
        Predicate<PropertyDeclarationSyntax> IsSupported = (p) =>
        {
            bool isPublic = p.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PublicKeyword));
            bool hasSetter = p.AccessorList?.Accessors
                .Any(accessor => accessor.Kind() == SyntaxKind.SetAccessorDeclaration) == true;

            return (isPublic, hasSetter) switch
            {
                (true, true) => true,
                (true, false) => false,
                (false, _) => false,
            };
        };

        return supportedMembers.OfType<PropertyDeclarationSyntax>()
            .Where(property => !IsSupported.Invoke(property))
            .Select(e => Diagnostic.Create(Rules.MustHavePublicSetter, e.GetLocation()));
    }

    private static IEnumerable<Diagnostic> GetTypeViolations(SyntaxNodeAnalysisContext context,
        List<MemberDeclarationSyntax> supportedMembers)
    {
        return supportedMembers.OfType<PropertyDeclarationSyntax>()
            .SelectMany(member => GetUnsupportedTypes(member, context.SemanticModel))
            .Where(typeSupport => !typeSupport.IsSupported)
            .Select(typeSupport => Diagnostic.Create(
                Rules.MustBeSupportedOpcUaType,
                typeSupport.Location,
                string.Join(", ", typeSupport.TypesWithError.Select(t => t.ToString())))
            );
    }

    private static MemberTypeSupport[] GetUnsupportedTypes(MemberDeclarationSyntax member, SemanticModel model)
    {
        return member switch
        {
            FieldDeclarationSyntax field => SupportedTypes.IsSupported(field.Declaration.Type, model)
                ? [MemberTypeSupport.Success(member)]
                : [MemberTypeSupport.Failure(member, field.Declaration.Type)],

            PropertyDeclarationSyntax property => SupportedTypes.IsSupported(property.Type, model)
                ? [MemberTypeSupport.Success(member)]
                : [MemberTypeSupport.Failure(member, property.Type)],

            MethodDeclarationSyntax method => GetMethodMemberSupport(method, model),

            var other => [MemberTypeSupport.Success(other!)]
        };
    }

    private static MemberTypeSupport[] GetMethodMemberSupport(MethodDeclarationSyntax method, SemanticModel model)
    {
        var unsupported = method.ParameterList.Parameters
            .Where(param => !SupportedTypes.IsSupported(param.Type!, model))
            .Select(e => (Type: e.Type!, Location: e.GetLocation()))
            .ToList();

        if (!SupportedTypes.IsSupported(method.ReturnType, model))
        {
            unsupported.Add((Type: method.ReturnType, method.ReturnType.GetLocation()));
        }

        if (unsupported.Any())
        {
            return unsupported
                .Select(err => MemberTypeSupport.Failure(err.Location, err.Type))
                .ToArray();
        }

        return [MemberTypeSupport.Success(method)];
    }
}