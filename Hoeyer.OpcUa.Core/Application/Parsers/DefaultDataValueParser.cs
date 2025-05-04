namespace Hoeyer.OpcUa.Core.Application.Parsers;

public sealed class DefaultDataValueParser<T>(T identity) : DataValueParser<T>(() => identity)
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