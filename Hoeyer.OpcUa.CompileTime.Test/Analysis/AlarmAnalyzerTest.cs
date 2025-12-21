using Hoeyer.OpcUa.Core.CompileTime;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

public sealed class AlarmAnalyzerTest() : DiagnosticAnalyzerTest(new AlarmAnalyser())
{
    private const string ENTITY_REFERENCE_REPLACEMENT_EXPRESSION = "!--XXX--!";
    private const string ENTITY_CLASS = "EntityClass";
    private const string NON_ENTITY_CLASS = "NonEntityClass";

    public const string CODE_TO_ANALYZE = $$"""
                                            using Hoeyer.OpcUa.Core;
                                            [OpcUaAlarm<!--XXX--!>]
                                            public enum Alarm
                                            {}
                                            [OpcUaEntity]
                                            public sealed record {{ENTITY_CLASS}}{}
                                            public sealed record {{NON_ENTITY_CLASS}}{}
                                            """;

    [Test]
    [DisplayName("When alarm attribute is not typed with entity, reports error")]
    public async Task WhenNotParameterizedWithEntity_ShouldReportError(CancellationToken token)
    {
        var sourceCode = CODE_TO_ANALYZE.Replace(ENTITY_REFERENCE_REPLACEMENT_EXPRESSION, NON_ENTITY_CLASS);
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsNotEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");
    }

    [Test]
    [DisplayName("When alarm attribute is typed with entity, does not reports error")]
    public async Task WhenParameterizedWithEntity_ShouldNotReportError(CancellationToken token)
    {
        var sourceCode = CODE_TO_ANALYZE.Replace(ENTITY_REFERENCE_REPLACEMENT_EXPRESSION, ENTITY_CLASS);
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");
    }
}