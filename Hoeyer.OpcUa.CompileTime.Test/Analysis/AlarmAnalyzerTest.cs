using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.CompileTime;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[InheritsTests]
public sealed class AlarmAnalyzerTest() : DiagnosticAnalyzerTest(new AlarmAnalyser())
{
    private const string REFERENCE_REPLACEMENT_EXPRESSION = "!--XXX--!";
    private const string ENTITY_CLASS = "EntityClass";
    private const string NON_ENTITY_CLASS = "NonEntityClass";

    public const string ATTRIBUTE_USAGE_EXAMPLE = $$"""
                                                    using Hoeyer.OpcUa.Core;
                                                    [OpcUaAlarm<!--XXX--!>]
                                                    public enum Alarm
                                                    {}
                                                    [OpcUaEntity]
                                                    public sealed record {{ENTITY_CLASS}}{}
                                                    public sealed record {{NON_ENTITY_CLASS}}{}
                                                    """;

    private const string ALARM_ANNOTATION =
        $"[{nameof(OpcUaAlarmTypeAttribute)}({nameof(AlarmValue)}.{nameof(AlarmValue.Discrete)})]";

    public const string ALARM_TYPE = $$"""
                                       using Hoeyer.OpcUa.Core;
                                       [OpcUaAlarm<{{ENTITY_CLASS}}>]
                                       public enum Alarm
                                       {
                                           {{REFERENCE_REPLACEMENT_EXPRESSION}}
                                           TestAlarm
                                           
                                       }
                                       [OpcUaEntity]
                                       public sealed record {{ENTITY_CLASS}}{}
                                       """;

    private static string GetIncorrectSourceCode() =>
        ATTRIBUTE_USAGE_EXAMPLE.Replace(REFERENCE_REPLACEMENT_EXPRESSION, NON_ENTITY_CLASS);

    private static string GetAlarmReferencingEntity() =>
        ATTRIBUTE_USAGE_EXAMPLE.Replace(REFERENCE_REPLACEMENT_EXPRESSION, ENTITY_CLASS);

    [Test]
    [DisplayName("When alarm attribute is not typed with entity, reports error")]
    public async Task WhenNotParameterizedWithEntity_ShouldReportError(CancellationToken token)
    {
        var sourceCode = GetIncorrectSourceCode();
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsNotEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");

        await Assert.That(analysis.Diagnostics.First().Severity).IsEqualTo(DiagnosticSeverity.Error);
    }

    [Test]
    [DisplayName("When reporting error, a location of the error is placed on the attribute usage")]
    public async Task When_ReportingError_LocationIsPresent(CancellationToken token)
    {
        var sourceCode = GetIncorrectSourceCode();
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);

        var errorLocation = analysis.Diagnostics.First().Location;
        await Assert.That(errorLocation).IsNotEqualTo(Location.None);
        await Assert.That(errorLocation.SourceSpan.Start).IsEqualTo(27);
        await Assert.That(errorLocation.SourceSpan.End).IsEqualTo(53);
    }

    [Test]
    [DisplayName("When alarm attribute is typed with entity, does not reports error")]
    public async Task WhenParameterizedWithEntity_ShouldNotReportError(CancellationToken token)
    {
        var sourceCode = GetAlarmReferencingEntity();
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");
    }

    [Test]
    [DisplayName($"When alarm enum fields are not annotated with {nameof(OpcUaAlarmTypeAttribute)}, reports error")]
    public async Task When_EnumIsNotAnnotatedWithAlarmType_ReportsError(CancellationToken token)
    {
        var sourceCode = ALARM_TYPE.Replace(REFERENCE_REPLACEMENT_EXPRESSION, "");
        var diagnostics = (await Driver.RunAnalyzerOn(sourceCode, token)).Diagnostics.ToList();
        await Assert.That(diagnostics).HasSingleItem();
        var diagnosis = diagnostics[0];
        await Assert.That(diagnosis.Severity).IsEqualTo(DiagnosticSeverity.Error);
        await Assert.That(diagnostics).HasSingleItem();
    }

    [Test]
    [DisplayName($"When alarm enum fields are annotated with {nameof(OpcUaAlarmTypeAttribute)}, does not report error")]
    public async Task When_EnumIsAnnotatedWithAlarmType_ReportsNoError(CancellationToken token)
    {
        var sourceCode = ALARM_TYPE.Replace(REFERENCE_REPLACEMENT_EXPRESSION, ALARM_ANNOTATION);
        var diagnostics = (await Driver.RunAnalyzerOn(sourceCode, token)).Diagnostics.ToList();
        await Assert.That(diagnostics).IsEmpty();
    }
}