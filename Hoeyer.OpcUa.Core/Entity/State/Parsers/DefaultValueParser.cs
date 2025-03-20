namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public sealed class DefaultValueParser<T>(T identity) : ValueParser<T>(() => identity)
{
    protected override bool TryGetTargetOrIdentity(object value, out T target)
    {
        if (value is T cast)
        {
            target = cast;
            return true;
        }

        target = identity;
        return false;
    }
}