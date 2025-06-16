using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.Simulation.SourceGeneration;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

public sealed class SimulationConfiguratorUsageAnalyserTest
{
    public const string EntityWithGeneratorClasses = """
                                                     using System;
                                                     using System.Collections.Generic;
                                                     using System.Threading.Tasks;
                                                     using Hoeyer.OpcUa.Core;
                                                     using Hoeyer.OpcUa.Server.Simulation.Api;
                                                     using Hoeyer.OpcUa.Server.Simulation.Services.Action;
                                                     using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
                                                     using Hoeyer.OpcUa.Server.Simulation;

                                                     namespace Hoeyer.opcUa.EntityDefinitions;

                                                     [OpcUaEntity]
                                                     public sealed class Gantry
                                                     {
                                                         public int IntValue { get; set; }
                                                         public string StringValue { get; set; }
                                                         public List<string> AList { get; set; }
                                                         public List<string> AAginList { get; set; }
                                                     }


                                                     [OpcUaEntityMethods<Gantry>]
                                                     public interface IGantryMethods
                                                     {
                                                         public Task IntegerInput(int q);
                                                         public Task<int> MultiInputIntReturn(int a, float b, List<int> i);
                                                         public Task<int> MoreMethods(int a, float b, float c, List<int> i);
                                                     }

                                                     [OpcMethodArguments<Gantry, IGantryMethods>("IntegerInput")]
                                                     public sealed record IntegerInputArgs
                                                     {
                                                     	public int Q { get; }
                                                     	public IntegerInputArgs(int q)
                                                     	{
                                                     		this.Q = q;
                                                     	}
                                                     }

                                                     [OpcMethodArgumentsAttribute<Gantry, IGantryMethods>("MultiInputIntReturn")]
                                                     public sealed record MultiInputIntReturnArgs
                                                     {
                                                     	public int A { get; }
                                                     	public float B { get; }
                                                     	public global::System.Collections.Generic.List<int> I { get; }
                                                     	public MultiInputIntReturnArgs(int a, float b, global::System.Collections.Generic.List<int> i)
                                                     	{
                                                     		this.A = a;
                                                     		this.B = b;
                                                     		this.I = i;
                                                     	}
                                                     }

                                                     """;

    private const string ActionSimulatorForVoidTask = """
                                                      public sealed class IntegerInputArgsActionSimulator : IActionSimulationConfigurator<Gantry, IntegerInputArgs>
                                                      {
                                                          public IEnumerable<ISimulationStep> ConfigureSimulation(IActionSimulationBuilder<Gantry, IntegerInputArgs> actionSimulationConfiguration) => [];
                                                      }
                                                      """;

    private const string ActionSimulatorForNonvoidTask = """
                                                         public sealed class IntegerInputArgsActionSimulator : IActionSimulationConfigurator<Gantry, MultiInputIntReturnArgs>
                                                         {
                                                             public IEnumerable<ISimulationStep> ConfigureSimulation(IActionSimulationBuilder<Gantry, MultiInputIntReturnArgs> actionSimulationConfiguration) => [];
                                                         }
                                                         """;

    private const string FuncSimulatorForNonVoidTask = """
                                                       public sealed class MultiInputIntReturnArgsFuncSimulator : IFunctionSimulationConfigurator<Gantry, MultiInputIntReturnArgs>
                                                       {
                                                           public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                               IFunctionSimulationBuilder<Gantry, MultiInputIntReturnArgs> actionSimulationConfiguration) =>
                                                               actionSimulationConfiguration.WithReturnValue((_) => 2);
                                                       }

                                                       """;

    private const string FuncSimulatorForVoidTask = """
                                                    public sealed class MultiInputIntReturnFuncSimulator : IActionSimulationConfigurator<Gantry, MultiInputIntReturnArgs>
                                                    {
                                                        public IEnumerable<ISimulationStep> ConfigureSimulation(IActionSimulationBuilder<Gantry, MultiInputIntReturnArgs> actionSimulationConfiguration) => [];
                                                    }
                                                    """;

    protected readonly SimulationConfiguratorUsageAnalyser Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);


    [Test]
    public async Task WhenAnalysing_ActionSimulatorForVoidTask_ShouldNotProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + ActionSimulatorForVoidTask;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsEmpty().Because(result.Diagnostics.ToCommaSeparatedString() +
                                                                " should not be present after running the analyser");
    }


    [Test]
    public async Task WhenAnalysing_ActionSimulatorForNonVoidTask_ShouldProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + ActionSimulatorForNonvoidTask;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulatorForNonVoidTask_ShouldNotProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorForNonVoidTask;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsEmpty();
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulatorForVoidTask_ShouldProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorForVoidTask;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }
}