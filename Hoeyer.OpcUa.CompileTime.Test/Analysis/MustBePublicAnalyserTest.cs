using Hoeyer.OpcUa.CompileTime.Analysis;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(MustBePublicAnalyser))]
[InheritsTests]
public class MustBePublicAnalyserTest : EntityAnalyzerTest<MustBePublicAnalyser>;