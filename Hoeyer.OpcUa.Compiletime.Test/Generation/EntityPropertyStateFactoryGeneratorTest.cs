using Hoeyer.OpcUa.Core.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Compiletime.Test.Generation;

[TestSubject(typeof(EntityPropertyStateFactoryGenerator))]
[InheritsTests]
public sealed class EntityPropertyStateFactoryGeneratorTest() : GeneratorWithEntityTargetTest(new EntityPropertyStateFactoryGenerator());