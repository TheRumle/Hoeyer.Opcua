using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.CompileTime.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.SymbolEqualityComparer;

namespace Hoeyer.OpcUa.Core.CompileTime;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AlarmAnalyser()
    : ConcurrentAnalyzer([Rules.MustMatchFieldType, Rules.IllegalRange])
{
    /// <inheritdoc />
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAttributeUsage, SyntaxKind.PropertyDeclaration);
    }

    private void AnalyzeAttributeUsage(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not PropertyDeclarationSyntax propertySyntax) //if null return
        {
            return;
        }

        var alarms = propertySyntax.GetOpcUaAlarmAttributes(context.SemanticModel).ToList();
        if (!alarms.Any())
        {
            return;
        }

        var errors = FindViolations(alarms, propertySyntax, context.SemanticModel);
        foreach (var diagnostic in errors)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static IEnumerable<Diagnostic> FindViolations(List<AttributeData> alarmAttributes,
        PropertyDeclarationSyntax propertySyntax,
        SemanticModel semanticModel)
    {
        var property = semanticModel.GetDeclaredSymbol(propertySyntax)!;
        return AlarmTypeMismatches(alarmAttributes, propertySyntax, property)
            .Union(LegalRangeMismatches(alarmAttributes, propertySyntax));
    }

    private static IEnumerable<Diagnostic> LegalRangeMismatches(
        List<AttributeData> alarmAttributes,
        PropertyDeclarationSyntax propertySyntax)
    {
        return alarmAttributes.Where(attr => attr.IsLegalRangeAlarm())
            .Select(attr =>
                (
                    attr,
                    min: (double)attr.ConstructorArguments[0].Value!,
                    max: (double)attr.ConstructorArguments[1].Value!
                )
            )
            .Where(tuple => tuple.min >= tuple.max)
            .Select(_ => Diagnostic.Create(Rules.IllegalRange, propertySyntax.GetLocation()))
            .ToList();
    }

    private static List<Diagnostic> AlarmTypeMismatches(
        IEnumerable<AttributeData> alarmAttributes,
        PropertyDeclarationSyntax propertySyntax,
        IPropertySymbol property)
    {
        return alarmAttributes
            .Where(attr => attr.AttributeClass is { IsGenericType: true, TypeArguments.Length: 1 })
            .Select(attr => (attrType: attr.AttributeClass!.TypeArguments[0], attribute: attr))
            .Where(attr => !Default.Equals(attr.attrType, property.Type))
            .Select(_ => Diagnostic.Create(Rules.MustMatchFieldType, propertySyntax.GetLocation()))
            .ToList();
    }
}