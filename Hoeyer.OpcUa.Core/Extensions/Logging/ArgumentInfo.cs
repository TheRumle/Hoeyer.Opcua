using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Extensions.Logging;

public class ArgumentInfo(string Name, object Value)
{
    public ArgumentInfo(Argument argument) : this(argument.Name, argument.Value)
    {
    }

    public object Value { get; } = Value;
    public string Name { get; } = Name;

    public static ArgumentInfo Of(Argument argument) => new(argument.Name, argument.Value);
}