using FluentResults;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.Analysis.Test;

public static class TheoryDataEntities
{
    /// <summary>
    /// Used to choose what data should be used in the test. 
    /// Represents valid test data, where errors are not expected.
    /// All data is constructed from test data classes annotated with <see cref="OpcUaEntityAttribute"/>;
    /// </summary>
    public class ValidData : TheoryData<SourceCodeInfo>
    {
        internal static readonly IEnumerable<SourceCodeInfo>
            Positives = GetSourceCodes(OpcTestEntities.PositiveEntities); 
        public ValidData()
        {
            foreach (var sourceCode in Positives)
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
    public class InvalidData :  TheoryData<SourceCodeInfo>
    {
        internal static readonly IEnumerable<SourceCodeInfo> Negatives = GetSourceCodes(OpcTestEntities.NegativeEntities);
        public InvalidData()
        {
            foreach (var sourceCode in Negatives)
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
            foreach (var sourceCode in InvalidData.Negatives.Union(ValidData.Positives))
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