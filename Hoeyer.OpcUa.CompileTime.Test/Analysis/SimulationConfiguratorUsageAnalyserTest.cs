using Hoeyer.Common.Extensions;
using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
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
                                                     using Hoeyer.OpcUa.Simulation.Api;
                                                     using Hoeyer.OpcUa.Simulation.Api.Configuration;
                                                     using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

                                                     namespace Hoeyer.OpcUa.EntityDefinitions;

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
                                                         public Task NoReturnValue(int q);
                                                         public Task<int> IntReturnValue();
                                                     }

                                                     [OpcMethodArguments<Gantry, IGantryMethods>("NoReturnValue")]
                                                     public sealed record NoReturnValueArgs
                                                     {
                                                     	public int Q { get; }
                                                     	public NoReturnValueArgs(int q)
                                                     	{
                                                     		this.Q = q;
                                                     	}
                                                     }

                                                     [OpcMethodArgumentsAttribute<Gantry, IGantryMethods>("IntReturnValue")]
                                                     public sealed record IntReturnValueArgs
                                                     {
                                                     	public IntReturnValueArgs()
                                                     	{
                                                     	}
                                                     }

                                                     """;

    private static readonly string ActionSimulationInterface = typeof(ISimulation<,>).Name.Split('`')[0];
    private static readonly string FunctionSimulationInterface = typeof(ISimulation<,,>).Name.Split('`')[0];

    private static readonly string SimulationBuilder = typeof(ISimulationBuilder<,>).Name.Split('`')[0];
    private static readonly string SimulationBuilderWithReturn = typeof(ISimulationBuilder<,,>).Name.Split('`')[0];

    private static readonly string ActionSimulator_NoReturnValue = $$"""
                                                                     public sealed class NoReturnValueActionSimulator : {{ActionSimulationInterface}}<Gantry, NoReturnValueArgs>
                                                                     {
                                                                         public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                             {{SimulationBuilder}}<Gantry, NoReturnValueArgs> actionSimulationConfiguration) => [];
                                                                     }

                                                                     """;

    private static readonly string ActionSimulatorFor_IntReturnValue = $$"""
                                                                         public sealed class IntReturnValueActionSimulator : {{ActionSimulationInterface}}<Gantry, IntReturnValueArgs>
                                                                         {
                                                                             public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                                 {{SimulationBuilder}}<Gantry, IntReturnValueArgs> actionSimulationConfiguration) => [];
                                                                         }
                                                                         """;

    private static readonly string FuncSimulatorFor_IntReturnValue = $$"""
                                                                       public sealed class
                                                                           IntReturnValueFuncSimulator : {{FunctionSimulationInterface}}<Gantry, IntReturnValueArgs, int>
                                                                       {
                                                                           public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                               {{SimulationBuilderWithReturn}}<Gantry, IntReturnValueArgs, int> actionSimulationConfiguration) =>
                                                                               actionSimulationConfiguration.WithReturnValue((_) => 2);
                                                                       }

                                                                       """;

    private static readonly string FuncSimulatorFor_NoReturnValue = $$"""
                                                                      public sealed class NoReturnValueFuncSimulator : {{FunctionSimulationInterface}}<Gantry, NoReturnValueArgs, int>
                                                                      {
                                                                          public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                              {{SimulationBuilderWithReturn}}<Gantry, NoReturnValueArgs, int> actionSimulationConfiguration) => [];
                                                                      }
                                                                      """;


    private static readonly string FuncSimulatorForIntTask_WrongReturnType = $$"""
                                                                               public sealed class
                                                                                   IntReturnValueButGivesStringFuncSimulator : {{FunctionSimulationInterface}}<Gantry, IntReturnValueArgs, string>
                                                                               {
                                                                                   public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                                       {{SimulationBuilderWithReturn}}<Gantry, IntReturnValueArgs, string> actionSimulationConfiguration) =>
                                                                                       actionSimulationConfiguration.WithReturnValue((_) => "2");
                                                                               }

                                                                               """;

    private static readonly string FuncSimulatorForIntTask_CorrectReturnType = $$"""
                                                                                 public sealed class
                                                                                     IntReturnValueButGivesStringFuncSimulator : {{FunctionSimulationInterface}}<Gantry, IntReturnValueArgs, int>
                                                                                 {
                                                                                     public IEnumerable<ISimulationStep> ConfigureSimulation(
                                                                                         {{SimulationBuilderWithReturn}}<Gantry, IntReturnValueArgs, int> actionSimulationConfiguration) =>
                                                                                         actionSimulationConfiguration.WithReturnValue((_) => 2);
                                                                                 }
                                                                                 """;


    private readonly SimulationConfiguratorUsageAnalyser Analyzer = new();
    private AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);


    [Test]
    public async Task WhenAnalysing_ActionSimulatorFor_MethodWith_NoReturn_ShouldNotProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + ActionSimulator_NoReturnValue;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsEmpty().Because(result.Diagnostics.ToCommaSeparatedString() +
                                                                " should not be present after running the analyser");
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulatorFor_CorrectGenericReturnValueArg_ShouldNotProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorForIntTask_CorrectReturnType;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsEmpty().Because(result.Diagnostics.ToCommaSeparatedString() +
                                                                " should not be present after running the analyser");
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulatorFor_MethodWith_IntReturn_ShouldNotProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorFor_IntReturnValue;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsEmpty().Because(result.Diagnostics.ToCommaSeparatedString() +
                                                                " should not be present after running the analyser");
    }


    [Test]
    public async Task WhenAnalysing_ActionSimulatorFor_MethodWith_IntReturn_ShouldProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + ActionSimulatorFor_IntReturnValue;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsNotEmpty().Because(" the code should produce diagnostics");
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulatorFor_MethodWith_NoReturnValue_ShouldProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorFor_NoReturnValue;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsNotEmpty().Because(" the code should produce diagnostics");
    }

    [Test]
    public async Task WhenAnalysing_FuncSimulator_IncorrectGenericReturnValueArg_ShouldProduceDiagnostics()
    {
        var sourceCode = EntityWithGeneratorClasses + FuncSimulatorForIntTask_WrongReturnType;
        AnalyzerResult result = await Driver.RunAnalyzerOn(sourceCode, CancellationToken.None);
        await Assert.That(result.Diagnostics).IsNotEmpty().Because(" the code should produce diagnostics");
    }
}