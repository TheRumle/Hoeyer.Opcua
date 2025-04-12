using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[TestSubject(typeof(EntityContainerGenerator))]
[ParallelLimiter<ParallelLimit>]
[InheritsTests]
public sealed class EntityContainerGeneratorTest : GeneratorTest<EntityContainerGenerator>;