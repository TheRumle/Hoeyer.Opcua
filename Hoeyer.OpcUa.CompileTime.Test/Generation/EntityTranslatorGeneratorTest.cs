using Hoeyer.OpcUa.Core.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[TestSubject(typeof(AgentTranslatorGenerator))]
[InheritsTests]
public sealed class AgentTranslatorGeneratorTest : GeneratorWithAgentTargetTest<AgentTranslatorGenerator>;