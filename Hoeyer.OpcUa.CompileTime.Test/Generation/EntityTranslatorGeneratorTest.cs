using Hoeyer.OpcUa.Core.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[TestSubject(typeof(EntityTranslatorGenerator))]
[InheritsTests]
public sealed class EntityTranslatorGeneratorTest : GeneratorWithEntityTargetTest<EntityTranslatorGenerator>;