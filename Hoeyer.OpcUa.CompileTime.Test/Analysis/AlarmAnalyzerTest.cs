using Hoeyer.OpcUa.Core.CompileTime;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[InheritsTests]
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

    private static string GetIncorrectSourceCode() =>
        CODE_TO_ANALYZE.Replace(ENTITY_REFERENCE_REPLACEMENT_EXPRESSION, NON_ENTITY_CLASS);

    private static string GetCorrectSourceCode() =>
        CODE_TO_ANALYZE.Replace(ENTITY_REFERENCE_REPLACEMENT_EXPRESSION, ENTITY_CLASS);

    [Test]
    [DisplayName("When alarm attribute is not typed with entity, reports error")]
    public async Task WhenNotParameterizedWithEntity_ShouldReportError(CancellationToken token)
    {
        var sourceCode = GetIncorrectSourceCode();
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsNotEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");
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
        var sourceCode = GetCorrectSourceCode();
        var analysis = await Driver.RunAnalyzerOn(sourceCode, token);
        await Assert.That(analysis.Diagnostics).IsEmpty()
            .Because("If the type parameter is not modelled as an entity, then an alarm cannot be created.");
    }
}