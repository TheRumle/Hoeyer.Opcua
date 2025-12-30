using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.CompileTime.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Hoeyer.OpcUa.Core.CompileTime.CodeDomain.WellKnown.FullyQualifiedAttribute;

namespace Hoeyer.OpcUa.Core.CompileTime;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AlarmAnalyser() : ConcurrentAnalyzer([Rules.IllegalRange])
{
    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAttributeUsage, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeAttributeUsage(SyntaxNodeAnalysisContext context)
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

        var errors = FindViolations(alarms, propertySyntax);
        foreach (var diagnostic in errors)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static IEnumerable<Diagnostic> FindViolations(List<AttributeData> alarms,
        PropertyDeclarationSyntax propertySyntax) =>
        Enumerable.Range(0, RangeThresholdMismatch(alarms)
                            + MinimumThresholdExceededRangeMismatch(alarms)
                            + MaximumThresholdExceededRangeMismatch(alarms))
            .Select(_ => Diagnostic.Create(Rules.IllegalRange, propertySyntax.GetLocation()));

    private static int RangeThresholdMismatch(
        List<AttributeData> alarmAttributes)
    {
        return ValidateRanges(
            alarmAttributes,
            attr => attr.AttributeClass!.IsType(LegalRangeAlarmAttribute),
            ExtractLegalRange,
            t => t.highHigh >= t.high &&
                 t.high >= t.low &&
                 t.low >= t.lowLow);
    }

    private static (double lowLow, double low, double high, double highHigh)
        ExtractLegalRange(AttributeData attr)
    {
        var args = attr.ConstructorArguments;

        return args.Length switch
        {
            6 => (
                low: (double)args[0].Value!,
                lowLow: (double)args[1].Value!,
                high: (double)args[2].Value!,
                highHigh: (double)args[3].Value!
            ),
            4 => (
                low: (double)args[0].Value!,
                lowLow: (double)args[0].Value!,
                high: (double)args[1].Value!,
                highHigh: (double)args[1].Value!
            ),
            var _ => throw new InvalidOperationException(
                $"The constructor of {LegalRangeAlarmAttribute.WithoutGlobalPrefix} " +
                $"had an unexpected amount of arguments: expected 6 or 4 but got {args.Length}")
        };
    }

    private static int MaximumThresholdExceededRangeMismatch(
        List<AttributeData> alarmAttributes)
    {
        return ValidateRanges(
            alarmAttributes,
            attr => attr.AttributeClass!.IsType(MaximumThresholdExceededAlarmAttribute),
            attr => (
                high: (double)attr.ConstructorArguments[0].Value!,
                highHigh: (double)attr.ConstructorArguments[1].Value!
            ),
            t => t.high <= t.highHigh);
    }

    private static int MinimumThresholdExceededRangeMismatch(
        List<AttributeData> alarmAttributes)
    {
        return ValidateRanges(
            alarmAttributes,
            attr => attr.AttributeClass!.IsType(MinimumThresholdExceededAlarmAttribute),
            attr => (
                lowLow: (double)attr.ConstructorArguments[0].Value!,
                low: (double)attr.ConstructorArguments[1].Value!
            ),
            t => t.lowLow <= t.low);
    }

    private static int ValidateRanges<TTuple>(
        IEnumerable<AttributeData> attributes,
        Func<AttributeData, bool> isAttribute,
        Func<AttributeData, TTuple> selector,
        Func<TTuple, bool> isValid)
    {
        return attributes
            .Where(attr => attr.AttributeClass is not null && isAttribute(attr))
            .Select(selector)
            .Count(tuple => !isValid(tuple));
    }
}