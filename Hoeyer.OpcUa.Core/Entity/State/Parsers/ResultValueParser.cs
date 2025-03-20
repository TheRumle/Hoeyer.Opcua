using FluentResults;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public sealed class ResultValueParser<T>() : ValueParser<Result<T>>(() => Result.Fail<T>("Could not extract target value"))
{
    protected override bool TryGetTargetOrIdentity(object value, out Result<T> target)
    {
        if (value is T cast)
        {
            target = cast;
            return true;
        }

        target = Identity.Invoke();
        return false;
    }
}