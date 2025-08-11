using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.CompileTime.Test.Drivers;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;
using Hoeyer.OpcUa.CompileTime.Test.Fixtures.Generators;
using Hoeyer.OpcUa.Core;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(AgentBehaviourAnalyzer))]
public class AgentBehaviourAnalyzerTest
{
    public const string NotAnnotated_PropertyAccessesTestAgent = """
                                                                 namespace Test;
                                                                 using System;
                                                                 using System.Collections.Generic;
                                                                 using Hoeyer.OpcUa.Core;
                                                                 public class PropertyAccessesTestAgent
                                                                 {
                                                                     public string S { get; set; }
                                                                 }
                                                                 """;

    protected readonly AgentBehaviourAnalyzer Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);

    private static string GetValidInterfaceFor(string agentName) =>
        $$"""
          namespace Test;
          using System;
          using System.Collections.Generic;
          using Hoeyer.OpcUa.Core;
          using System.Threading.Tasks;
          [{{nameof(OpcUaAgentMethodsAttribute<object>)}}<{{agentName}}>]
          public interface FuncReferencingSelf
          {
              public Task<List<int>> ListMethod(bool a, int b, int c);
              public Task<int> function(int a, int b, int c);
              public Task<int> function1(int a, int b, int c);
              public Task<int> function2(int a, int b, int c);
              public Task functionBool(bool a, int b, int c);
          }
          """;

    [Test]
    public async Task WhenGiven_NonOpcAgentClass_ReportsDiagnostic()
    {
        var agentName = "PropertyAccessesTestAgent";
        var interfaceDefinition = GetValidInterfaceFor(agentName);
        var data = new AgentAndServiceSourceCode("TestInterface", agentName,
            NotAnnotated_PropertyAccessesTestAgent, interfaceDefinition);


        var result = await Driver.RunAnalyzerOn(data);
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }

    [Test]
    [ValidAgentSourceCodeGenerator]
    public async Task WhenGiven_ValidAgent_ValidInterface_NoDiagnosticPresent(AgentSourceCode sourceCode)
    {
        var interfaceDefinition = GetValidInterfaceFor(sourceCode.Type);
        var data = new AgentAndServiceSourceCode("TestInterface", sourceCode.Type,
            sourceCode.SourceCodeString, interfaceDefinition);

        AnalyzerResult result = await Driver.RunAnalyzerOn(data, CancellationToken.None);

        await Assert.That(result.Diagnostics).IsEmpty();
    }
}