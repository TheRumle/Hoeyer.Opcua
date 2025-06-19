using System.Collections;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public readonly record struct ArgumentInfo(string Name, string Params)
{
    public ArgumentInfo(Argument argument) : this(argument.Name, GetArgumentValueString(argument))
    {
    }

    public string Params { get; } = Params;
    public string Name { get; } = Name;

    private static string GetArgumentValueString(Argument argument)
    {
        var value = argument.Value;
        if (value is null)
        {
            return "'null'";
        }

        return value is IEnumerable c
            ? $"[\n" + string.Join(",\n ", c)
            : value.ToString();
    }

    public static ArgumentInfo Of(Argument argument) => new(argument);
}