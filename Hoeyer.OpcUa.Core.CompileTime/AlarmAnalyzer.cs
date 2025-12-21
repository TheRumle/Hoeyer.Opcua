using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.CompileTime.CodeDomain;
using Hoeyer.OpcUa.Core.CompileTime.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Core.CompileTime;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AlarmAnalyser()
    : ConcurrentAnalyzer([Rules.MustBeOpcEntityArgument, Rules.MustBeAnnotatedWithAlarmType])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAttributeUseage, SyntaxKind.EnumDeclaration);
    }

    private void AnalyzeAttributeUseage(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not EnumDeclarationSyntax typeSyntax ||
            !typeSyntax.IsAnnotatedAsOpcUaAlarm(context.SemanticModel))
        {
            return;
        }

        var errors = FindViolations(typeSyntax, context);
        foreach (var diagnostic in errors)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static IEnumerable<Diagnostic> FindViolations(
        EnumDeclarationSyntax enumSyntax,
        SyntaxNodeAnalysisContext context
    ) => AlarmAttributeTargetsEntity(enumSyntax, context)
        .Union(AlarmFieldsAreAnnotatedWithValue(enumSyntax, context));

    private static IEnumerable<Diagnostic> AlarmFieldsAreAnnotatedWithValue(EnumDeclarationSyntax enumDecl,
        SyntaxNodeAnalysisContext context)
    {
        return enumDecl.Members
            .Where(member => !member.AttributeLists //where attribute is not annotated with alarm type
                .SelectMany(al => al.Attributes)
                .Any(attr => context.SemanticModel.GetSymbolInfo(attr).Symbol is { ContainingType: var containingType }
                             && containingType.GloballyQualifiedNonGeneric()
                                 .Equals(WellKnown.FullyQualifiedAttribute.AlarmTypeAttribute.WithGlobalPrefix))
            )
            .Select(member =>
                Diagnostic.Create(Rules.MustBeAnnotatedWithAlarmType, member.GetLocation()));
    }

    private static IEnumerable<Diagnostic> AlarmAttributeTargetsEntity(EnumDeclarationSyntax enumSyntax,
        SyntaxNodeAnalysisContext context)
    {
        var attribute = enumSyntax.GetOpcUaAlarmAttribute(context.SemanticModel);
        if (attribute is null)
        {
            return [];
        }

        var typeArg = attribute?.AttributeClass?.TypeArguments.FirstOrDefault();
        if (typeArg is null)
        {
            return [];
        }

        if (!typeArg.IsAnnotatedAsOpcUaEntity())
        {
            var attrSyntax = attribute?
                .ApplicationSyntaxReference?
                .GetSyntax(context.CancellationToken) as AttributeSyntax;

            var location = attrSyntax?.GetLocation() ?? enumSyntax.GetLocation();
            return [Diagnostic.Create(Rules.MustBeOpcEntityArgument, location)];
        }

        return [];
    }
}