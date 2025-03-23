using Hoeyer.Opc.Ua.Test.TUnit;
using Hoeyer.OpcUa.Entity.CompileTime.Test.Generation;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Entity.Analysis.Test.Generation;

[TestSubject(typeof(EntityNodeFactoryGenerator))]
[ParallelLimiter<ParallelLimit>]
[InheritsTests]
public sealed class EntityNodeFactoryGeneratorTest : GeneratorTest<EntityNodeFactoryGenerator>;