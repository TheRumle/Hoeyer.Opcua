using Hoeyer.OpcUa.Client.SourceGeneration.Generation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.CompileTime.Test.Generation;

[TestSubject(typeof(RemoteMethodCallerGenerator))]
[InheritsTests]
public sealed class RemoteMethodCallerGeneratorTest()
    : GeneratorWithEntityMethodsTargetTest(new RemoteMethodCallerGenerator());