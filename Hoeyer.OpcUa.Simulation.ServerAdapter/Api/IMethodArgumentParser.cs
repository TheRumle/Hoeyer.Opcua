using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter.Api;

public interface IMethodArgumentParser<out TMethodArgs>
{
    /// <summary>
    /// Parses the input argument list into an instance of <typeparamref name="TMethodArgs"/>.
    /// </summary>
    /// <param name="inputArguments">The list of input arguments to be parsed.</param>
    /// <returns>An instance of <typeparamref name="TMethodArgs"/> representing the parsed arguments.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of input arguments does not match the number of properties in <typeparamref name="TMethodArgs"/>,
    /// or when the argument structure could not be parsed successfully.
    /// </exception>
    TMethodArgs ParseToArgsStructure(IList<object> inputArguments);
}