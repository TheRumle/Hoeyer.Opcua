using System.Diagnostics.CodeAnalysis;
using TUnit.Core.Interfaces;

namespace Hoeyer.OpcUa.EndToEndTest.Configuration;

public class ParallelLimit : IParallelLimit
{
    private const int LIMIT = 20;

    [SuppressMessage("Design", "S2325",
        Justification = "IParallelLimit must be implemented and method cannot be static.")]
    public int Limit => LIMIT;
}