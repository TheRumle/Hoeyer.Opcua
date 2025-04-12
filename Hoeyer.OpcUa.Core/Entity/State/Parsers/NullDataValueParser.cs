namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public class NullDataValueParser<T>() : DataValueParser<T?>(() => default)
{
    protected override bool TryGetTargetOrIdentity(object value, out T? target)
    {
        if (value is T cast)
        {
            target = cast;
            return true;
        }

        target = default;
        return false;
    }
}