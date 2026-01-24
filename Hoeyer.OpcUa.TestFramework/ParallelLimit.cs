using System.Diagnostics.CodeAnalysis;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.Test;

public class ParallelLimit : IParallelLimit
{
    private const int DefaultLimit = 10;
    private static readonly int _limit = GetLimitFromEnvironment();

    [SuppressMessage("Design", "S2325",
        Justification = "IParallelLimit must be implemented and method cannot be static.")]
    public int Limit => _limit;

    private static int GetLimitFromEnvironment()
    {
        var value = Environment.GetEnvironmentVariable("TUNIT_PARALLEL_LIMIT");

        return int.TryParse(value, out var limit) && limit > 0
            ? limit
            : DefaultLimit;
    }
}