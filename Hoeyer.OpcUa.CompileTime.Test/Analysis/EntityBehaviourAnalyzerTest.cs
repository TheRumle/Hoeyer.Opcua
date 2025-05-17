using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;

[TestSubject(typeof(EntityBehaviourAnalyzer))]
public class EntityBehaviourAnalyzerTest
{
    public const string NotAnnotated_PropertyAccessesTestEntity = """
                                                                  namespace Test;
                                                                  using System;
                                                                  using System.Collections.Generic;
                                                                  using Hoeyer.OpcUa.Core;
                                                                  public class PropertyAccessesTestEntity
                                                                  {
                                                                      public string S { get; set; }
                                                                  }
                                                                  """;

    protected readonly EntityBehaviourAnalyzer Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);

    private static string GetValidInterfaceFor(string entityName) =>
        $$"""
          namespace Test;
          using System;
          using System.Collections.Generic;
          using Hoeyer.OpcUa.Core;
          using System.Threading.Tasks;
          [{{nameof(OpcUaEntityMethodsAttribute<object>)}}<{{entityName}}>]
          public interface FuncReferencingSelf
          {
              public Task<int> function(int a, int b, int c);
              public Task<int> function1(int a, int b, int c);
              public Task<int> function2(int a, int b, int c);
              public Task functionBool(bool a, int b, int c);
          }
          """;

    [Test]
    public async Task WhenGiven_NonOpcEntityClass_ReportsDiagnostic()
    {
        var entityName = "PropertyAccessesTestEntity";
        var interfaceDefinition = GetValidInterfaceFor(entityName);
        var data = new EntityAndServiceSourceCode("TestInterface", entityName,
            NotAnnotated_PropertyAccessesTestEntity, interfaceDefinition);


        var result = await Driver.RunAnalyzerOn(data);
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }

    [Test]
    [ValidEntitySourceCodeGenerator]
    public async Task WhenGiven_ValidEntity_ValidInterface_NoDiagnosticPresent(EntitySourceCode sourceCode)
    {
        var interfaceDefinition = GetValidInterfaceFor(sourceCode.Type);
        var data = new EntityAndServiceSourceCode("TestInterface", sourceCode.Type,
            sourceCode.SourceCodeString, interfaceDefinition);

        AnalyzerResult result = await Driver.RunAnalyzerOn(data, CancellationToken.None);

        await Assert.That(result.Diagnostics).IsEmpty();
    }
}