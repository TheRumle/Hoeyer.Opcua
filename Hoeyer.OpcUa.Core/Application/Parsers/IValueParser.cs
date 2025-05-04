namespace Hoeyer.OpcUa.Core.Application.Parsers;

public interface IValueParser<in TSource, out TTarget>
{
    public TTarget Parse(TSource dataValue);
}