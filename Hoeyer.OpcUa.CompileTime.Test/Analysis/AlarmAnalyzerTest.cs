using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.CompileTime;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[InheritsTests]
[TestSubject(typeof(AlarmAnalyser))]
[TestSubject(typeof(LegalRangeAlarmAttribute))]
[TestSubject(typeof(MinimumThresholdExceededAlarmAttribute<>))]
[TestSubject(typeof(MaximumThresholdExceededAlarmAttribute<>))]
public sealed class AlarmAnalyzerTest() : DiagnosticAnalyzerTest(new AlarmAnalyser())
{
    private const string ENTITY_CLASS = "EntityClass";

    [Test]
    [DisplayName(
        $"({nameof(MaximumThresholdExceededAlarmAttribute<int>)}) When using alarm with generic argument double on property of type int, an error is reported")]
    public async Task When_MaxThreshold_AlarmGenericArg_DoesNotMatchField_ReportsError(CancellationToken token)
    {
        const string maximumThresholdAlarmClass = $$"""
                                                    using Hoeyer.OpcUa.Core;
                                                    public sealed record {{ENTITY_CLASS}}
                                                    {
                                                        [{{nameof(MaximumThresholdExceededAlarmAttribute<int>)}}<double>(11, "IntValueAlarm", AlarmSeverity.Critical)]
                                                        public int MyInt {get; set;}
                                                    }
                                                    """;
        var diagnostics = (await Driver.RunAnalyzerOn(maximumThresholdAlarmClass, token)).Diagnostics.ToList();
        await AssertHasOnlyError(diagnostics, Rules.MustMatchFieldType);
    }


    [Test]
    [DisplayName(
        $"({nameof(MaximumThresholdExceededAlarmAttribute<int>)}) When using alarm with generic argument int on property of type int, no error")]
    public async Task When_MaxThreshold_AlarmGenericArg_DoesMatchesField_DoesNotReportError(CancellationToken token)
    {
        const string nonEntityAnnotatedClass = $$"""
                                                 using Hoeyer.OpcUa.Core;
                                                 public sealed record {{ENTITY_CLASS}}
                                                 {
                                                     [{{nameof(MaximumThresholdExceededAlarmAttribute<int>)}}<int>(11, "IntValueAlarm", AlarmSeverity.Critical)]
                                                     public int MyInt {get; set;}
                                                 }
                                                 """;
        var diagnostics = (await Driver.RunAnalyzerOn(nonEntityAnnotatedClass, token)).Diagnostics.ToList();
        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    [DisplayName(
        $"({nameof(MinimumThresholdExceededAlarmAttribute<int>)}) When using alarm with generic argument double on property of type int, an error is reported")]
    public async Task When_MinThreshold_AlarmGenericArg_DoesNotMatchField_ReportsError(CancellationToken token)
    {
        const string maximumThresholdAlarmClass = $$"""
                                                    using Hoeyer.OpcUa.Core;
                                                    public sealed record {{ENTITY_CLASS}}
                                                    {
                                                        [{{nameof(MinimumThresholdExceededAlarmAttribute<int>)}}<double>(11, "IntValueAlarm", AlarmSeverity.Critical)]
                                                        public int MyInt {get; set;}
                                                    }
                                                    """;
        var diagnostics = (await Driver.RunAnalyzerOn(maximumThresholdAlarmClass, token)).Diagnostics.ToList();
        await AssertHasOnlyError(diagnostics, Rules.MustMatchFieldType);
    }


    [Test]
    [DisplayName(
        $"({nameof(MinimumThresholdExceededAlarmAttribute<int>)})  When using alarm with generic argument int on property of type int, no error")]
    public async Task When_MinThreshold_AlarmGenericArg_DoesMatchesField_DoesNotReportError(CancellationToken token)
    {
        const string nonEntityAnnotatedClass = $$"""
                                                 using Hoeyer.OpcUa.Core;
                                                 public sealed record {{ENTITY_CLASS}}
                                                 {
                                                     [{{nameof(MinimumThresholdExceededAlarmAttribute<int>)}}<int>(11, "IntValueAlarm", AlarmSeverity.Critical)]
                                                     public int MyInt {get; set;}
                                                 }
                                                 """;
        var diagnostics = (await Driver.RunAnalyzerOn(nonEntityAnnotatedClass, token)).Diagnostics.ToList();
        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    [DisplayName($"When using {nameof(LegalRangeAlarmAttribute)} with range of size 0, reports error")]
    public async Task When_LegalRangeAlarm_MustConstructValidRange(CancellationToken token)
    {
        const string nonEntityAnnotatedClass = $$"""
                                                 using Hoeyer.OpcUa.Core;
                                                 public sealed record {{ENTITY_CLASS}}
                                                 {
                                                     [{{nameof(LegalRangeAlarmAttribute)}}(11, 11, "IntValueAlarm", AlarmSeverity.Critical)]
                                                     public int MyInt {get; set;}
                                                 }
                                                 """;
        var diagnostics = (await Driver.RunAnalyzerOn(nonEntityAnnotatedClass, token)).Diagnostics.ToList();
        await AssertHasOnlyError(diagnostics, Rules.IllegalRange);
    }

    [Test]
    [DisplayName($"When using {nameof(LegalRangeAlarmAttribute)} with negative range, reports error")]
    public async Task When_LegalRangeAlarm_MustConstructValidRange_negative_reports(CancellationToken token)
    {
        const string nonEntityAnnotatedClass = $$"""
                                                 using Hoeyer.OpcUa.Core;
                                                 public sealed record {{ENTITY_CLASS}}
                                                 {
                                                     [{{nameof(LegalRangeAlarmAttribute)}}(11, 9, "IntValueAlarm", AlarmSeverity.Critical)]
                                                     public int MyInt {get; set;}
                                                 }
                                                 """;
        var diagnostics = (await Driver.RunAnalyzerOn(nonEntityAnnotatedClass, token)).Diagnostics.ToList();
        await Assert.That(diagnostics.Count).IsEqualTo(1);
        await AssertHasOnlyError(diagnostics, Rules.IllegalRange);
    }

    [Test]
    [DisplayName($"When using {nameof(LegalRangeAlarmAttribute)} with positive range, no error is reported")]
    public async Task When_LegalRangeAlarm_WithValidRange_DoesNotReportError(CancellationToken token)
    {
        const string nonEntityAnnotatedClass = $$"""
                                                 using Hoeyer.OpcUa.Core;
                                                 public sealed record {{ENTITY_CLASS}}
                                                 {
                                                     [{{nameof(LegalRangeAlarmAttribute)}}(11, 13, "IntValueAlarm", AlarmSeverity.Critical)]
                                                     public int MyInt {get; set;}
                                                 }
                                                 """;
        var diagnostics = (await Driver.RunAnalyzerOn(nonEntityAnnotatedClass, token)).Diagnostics.ToList();
        await Assert.That(diagnostics.Count).IsEqualTo(0);
    }


    private static async Task AssertHasOnlyError(List<Diagnostic> diagnostics, DiagnosticDescriptor descriptor)
    {
        await Assert.That(diagnostics.Count).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo(descriptor.Id);
        await Assert.That(diagnostics[0].Severity).IsEqualTo(DiagnosticSeverity.Error);
    }
}