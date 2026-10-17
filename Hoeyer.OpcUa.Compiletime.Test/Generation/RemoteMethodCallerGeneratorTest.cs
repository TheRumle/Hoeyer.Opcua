using Hoeyer.OpcUa.Client.SourceGeneration.Generation;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;

namespace Hoeyer.OpcUa.Compiletime.Test.Generation;

[TestSubject(typeof(RemoteMethodCallerGenerator))]
[InheritsTests]
public sealed class RemoteMethodCallerGeneratorTest()
    : GeneratorWithEntityMethodsTargetTest(new RemoteMethodCallerGenerator())
{
    private const string GantryMethods = """
                                  using Hoeyer.OpcUa.Core;
                                  namespace MyNamespace;
                                  public enum Position
                                  {
                                      OverThere,
                                      OverHere,
                                      OnTheMoon,
                                      SanDiego,
                                      Mexico,
                                      Submarine,
                                      TheSecretUndergroundLab
                                  }

                                  [OpcUaEntity]
                                  public sealed class Gantry
                                  {
                                      public required int IntValue { get; set; }
                                      public required Guid HeldContainer { get; set; }
                                      public required bool Occupied { get; set; }
                                      public required string StringValue { get; set; }
                                      public required List<string> ListValue { get; set; }
                                  }
                                  
                                  
                                  [OpcUaEntityMethods<Gantry>]
                                  public interface IGantryMethods
                                  {
                                      Task ChangePosition(Position position);
    
                                      Task<int> PlaceContainer(Position position);
                                      Task PickUpContainer(Position position);
                                      public Task AssignContainer(Guid guid);
                                      public Task AssignContainers(List<Guid> guids);
                                      public Task X(int a, int b);
                                  }
                                  """;

    [Test]
    public async Task CanGenerateForGantry()
    {
        var result = TestDriver.RunGeneratorOn(GantryMethods);
        await Assert.That(result.GeneratedTrees).IsNotEmpty().Because("source code should be generated.");

        var syntaxTree = CSharpSyntaxTree.ParseText(result.SourceCode);
        var diagnostics = syntaxTree.GetDiagnostics().ToList();
        await Assert.That(diagnostics).IsEmpty()
            .Because(
                $" the generated syntax trees should not have compilation errors: {string.Join('\n', diagnostics)}");
    }
}