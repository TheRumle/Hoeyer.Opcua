using System.Runtime.Serialization;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Xunit;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

[Serializable]
public record SourceCodeInfo([field: NonSerialized] Type Type, string SourceCodeString)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return Type.Name;
    }
};

public static class TestEntities
{
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents valid test data, where errors are not expected.
    /// All data is constructed from test data classes annotated with <see cref="OpcUaEntityAttribute"/>;
    /// </summary>
    public class ValidData : TheoryData<SourceCodeInfo>
    {
        public ValidData()
        {
            foreach (var sourceCode in GetSourceCodes(OpcTestEntities.PositiveEntities))
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
    public class NegativeData :  TheoryData<SourceCodeInfo>
    {
        public NegativeData()
        {
            foreach (var sourceCode in GetSourceCodes(OpcTestEntities.NegativeEntities))
            {
                Add(sourceCode);   
            }
        }
    }
    
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents valid test data, where errors are not expected.
    /// </summary>
    public class AllData :  TheoryData<SourceCodeInfo>
    {
        public AllData()
        {
            foreach (var sourceCode in GetSourceCodes(OpcTestEntities.NegativeEntities.Union(OpcTestEntities.NegativeEntities)))
            {
                Add(sourceCode);   
            }
        }
    }
    

    private static IEnumerable<SourceCodeInfo> GetSourceCodes(IEnumerable<Type> types)
    {
        return types
            .Select(OpcEntityResourceLoader.LoadTypeAsResourceString)
            .Merge()
            .GetOrThrow(e => new InvalidDataException(e.Message))
            .Select(e=> new SourceCodeInfo(e.Type, e.TypeAsString));
    }
}
