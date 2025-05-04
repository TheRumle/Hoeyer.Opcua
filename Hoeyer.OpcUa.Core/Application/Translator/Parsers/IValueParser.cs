namespace Hoeyer.OpcUa.Core.Application.Translator.Parsers;

public interface IValueParser<in TSource, out TTarget>
{
    public TTarget Parse(TSource dataValue);
}