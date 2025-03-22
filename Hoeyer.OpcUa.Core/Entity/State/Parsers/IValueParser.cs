namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public interface IValueParser<in TSource, out TTarget>
{
    public TTarget Parse(TSource dataValue);
}