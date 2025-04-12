using Hoeyer.OpcUa.CompileTime.Analysis;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(PropertiesMustNotBeNullableAnalyser))]
[InheritsTests]
public class PropertiesMustNotBeNullableAnalyserTest : AnalyzerTest<PropertiesMustNotBeNullableAnalyser>;