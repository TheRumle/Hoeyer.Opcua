using System.Collections;
using FluentResults;
using Hoeyer.Common.Extensions.Functional;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public class CorrectEntitySourceCode : IEnumerable<string[]>
{
    private static async Task<Result<IEnumerable<string>>> PositiveEntitiesAsync() => await EntityData.PositiveEntities.Select(async type => await EntityResourceLoader.LoadTypeAsResourceAsync(type)).Combine();
    

    /// <inheritdoc />
    public IEnumerator<string[]> GetEnumerator()    
    {
        var testEntitiesResult = PositiveEntitiesAsync().Result;
        if (testEntitiesResult.IsFailed) throw new ArgumentException(testEntitiesResult.Errors.Aggregate("Failed to load entity source code! \n", (prev, next) => prev + Environment.NewLine + next));
        foreach (var item in testEntitiesResult.Value) yield return [item];
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}