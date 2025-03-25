using Hoeyer.OpcUa.CompileTime.Analysis;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(PropertyMustBeOfSupportedTypeAnalyser))]
[InheritsTests]
public class PropertyMustBeOfSupportedTypeAnalyserTest : AnalyzerTest<PropertyMustBeOfSupportedTypeAnalyser>;