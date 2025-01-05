using System.Reflection;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Xunit;
using Xunit.Sdk;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;



public static class TestEntities
{
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents valid test data, where errors are not expected.
    /// All data is constructed from test data classes annotated with <see cref="OpcUaEntityAttribute"/>;
    /// </summary>
    public class ValidData : TheoryData<string>
    {
        public ValidData()
        {
            foreach (var sourceCode in GetSourceCodes(TestEntityTypes.PositiveEntities))
            {
                Add(sourceCode);   
            }
        }
    }
    
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents invalid test data, where errors are expected.
    /// All data is constructed from test data classes annotated with <see cref="OpcUaEntityAttribute"/>;
    /// </summary>
    public class NegativeData :  TheoryData<string>
    {
        public NegativeData()
        {
            foreach (var sourceCode in GetSourceCodes(TestEntityTypes.NegativeEntities))
            {
                Add(sourceCode);   
            }
        }
    }
    
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents valid test data, where errors are not expected.
    /// </summary>
    public class AllData :  TheoryData<string>
    {
        public AllData()
        {
            foreach (var sourceCode in GetSourceCodes(TestEntityTypes.NegativeEntities.Union(TestEntityTypes.NegativeEntities)))
            {
                Add(sourceCode);   
            }
        }
    }
    

    private static IEnumerable<string> GetSourceCodes(IEnumerable<Type> types)
    {
        return types
            .Select(OpcEntityResourceLoader.LoadTypeAsResourceString)
            .Merge()
            .GetOrThrow(e => new InvalidDataException(e.Message));
    }
}