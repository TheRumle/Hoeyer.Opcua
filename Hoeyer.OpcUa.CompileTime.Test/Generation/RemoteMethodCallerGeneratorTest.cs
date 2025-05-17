using Hoeyer.OpcUa.Client.SourceGeneration;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[TestSubject(typeof(RemoteMethodCallerGenerator))]
[InheritsTests]
public sealed class RemoteMethodCallerGeneratorTest : GeneratorWithEntityTargetTest<RemoteMethodCallerGenerator>;