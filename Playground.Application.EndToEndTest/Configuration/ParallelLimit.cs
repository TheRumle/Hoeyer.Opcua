using System.Diagnostics.CodeAnalysis;
using TUnit.Core.Interfaces;

namespace Playground.Application.EndToEndTest.Configuration;

public class ParallelLimit : IParallelLimit
{
    private const int LIMIT = 100;

    [SuppressMessage("Design", "S2325",
        Justification = "IParallelLimit must be implemented and method cannot be static.")]
    public int Limit => LIMIT;
}