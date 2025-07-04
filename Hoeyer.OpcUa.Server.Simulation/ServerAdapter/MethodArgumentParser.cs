using System;
using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Simulation.Api;

namespace Hoeyer.OpcUa.Server.Simulation.ServerAdapter;

/// <summary>
/// Parses a list of method arguments into a strongly-typed argument structure of type <typeparamref name="TMethodArgs"/>.
/// </summary>
/// <typeparam name="TMethodArgs">The type representing the structure of method arguments.</typeparam>
internal sealed class MethodArgumentParser<TMethodArgs>(IEntityMethodArgTranslator<TMethodArgs> argsMapper)
    : IMethodArgumentParser<TMethodArgs>
{
    private static readonly int _numberOfArgs = typeof(TMethodArgs).GetProperties().Length;

    /// <inheritdoc/>
    public TMethodArgs ParseToArgsStructure(IList<object> inputArguments)
    {
        var argumentStructure = argsMapper.Map(inputArguments);
        if (inputArguments.Count != _numberOfArgs)
        {
            throw new ArgumentException(
                "The method was called with invalid number variables and could not be processed.");
        }

        if (Equals(argumentStructure, default(TMethodArgs)))
        {
            throw new ArgumentException("The argument structure could not be parsed: " +
                                        string.Join(",", inputArguments));
        }

        return argumentStructure!;
    }
}