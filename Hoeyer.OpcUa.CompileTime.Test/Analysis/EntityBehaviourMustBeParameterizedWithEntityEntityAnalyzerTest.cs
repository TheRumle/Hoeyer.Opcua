using Hoeyer.OpcUa.CompileTime.Analysis;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Drivers;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.CompileTime.Test.Analysis;


[TestSubject(typeof(EntityBehaviourMustBeParameterizedWithEntityAnalyzer))]
public class
    EntityBehaviourMustBeParameterizedWithEntityEntityAnalyzerTest
{
    protected readonly EntityBehaviourMustBeParameterizedWithEntityAnalyzer Analyzer = new();
    protected AnalyzerTestDriver<DiagnosticAnalyzer> Driver => new(Analyzer, Console.WriteLine);
    private static string GetValidInterfaceFor(string entityName) =>
        $$"""
          namespace Test;
          using System;
          using System.Collections.Generic;
          using Hoeyer.OpcUa.Core;
          [OpcUaEntityMethods(typeof({{entityName}}))]
          public interface FuncReferencingSelf
          {
              public int function(int a, int b, int c);
              public int function1(int a, int b, int c);
              public int function2(int a, int b, int c);
              public void functionBool(bool a, int b, int c);
          }
          """;
    
    
    public const string NotAnnotated_PropertyAccessesTestEntity =  """
                                                      namespace Test;
                                                      using System;
                                                      using System.Collections.Generic;
                                                      using Hoeyer.OpcUa.Core;
                                                      public class PropertyAccessesTestEntity
                                                      {
                                                          public string S { get; set; }
                                                      }
                                                      """;
    
    [Test]
    public async Task WhenGiven_NonOpcEntityClass_ReportsDiagnostic()
    {
        var entityName = "PropertyAccessesTestEntity";
        var interfaceDefinition = GetValidInterfaceFor(entityName);
        var data = new EntityAndServiceSourceCode("TestInterface", entityName,
            NotAnnotated_PropertyAccessesTestEntity + "\n" + interfaceDefinition);
        
        
        var result = await Driver.RunAnalyzerOn(data);
        await Assert.That(result.Diagnostics).IsNotEmpty();
    }
    
    [Test]
    [ValidEntitySourceCodeGenerator]
    public async Task WhenGiven_ValidEntity_ValidInterface_NoDiagnosticPresent(EntitySourceCode sourceCode)
    {
        var interfaceDefinition = GetValidInterfaceFor(sourceCode.Type);
        var data = new EntityAndServiceSourceCode("TestInterface", sourceCode.Type,
            sourceCode.SourceCodeString + "\n" + interfaceDefinition);
        
        var result = await Driver.RunAnalyzerOn(data, CancellationToken.None) ;
        
        await Assert.That(result.Diagnostics).IsEmpty();
    }
    
    
    
}