using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Entity.State.Parsers;

public static class DataValueParsers
{
    public static ResultValueParser<int>  NewIntResultValueParser => new();
    public static DefaultValueParser<int>  NewIntValueParser => new(0);
    public static ResultValueParser<float>  NewFloatResultParser => new();
    public static DefaultValueParser<float>  NewFloatParser => new(0f);
    public static ResultValueParser<double>  NewDoubleResultParser => new();
    public static DefaultValueParser<double>  NewDoubleParser => new(0.0);
    public static ResultValueParser<decimal>  NewDecimalResultParser => new();
    public static DefaultValueParser<decimal>  NewDecimalParser => new(0m);
    public static ResultValueParser<long>  NewLongResultParser => new();
    public static DefaultValueParser<long>  NewLongParser => new(0L);
    public static ResultValueParser<short>  NewShortResultParser => new();
    public static DefaultValueParser<short>  NewShortParser => new(0);
    public static ResultValueParser<byte>  NewByteResultParser => new();
    public static DefaultValueParser<byte>  NewByteParser => new(0);
    public static ResultValueParser<sbyte>  NewSByteResultParser => new();
    public static DefaultValueParser<sbyte>  NewSByteParser => new(0);
    public static ResultValueParser<ushort>  NewUShortResultParser => new();
    public static DefaultValueParser<ushort>  NewUShortParser => new(0);
    public static ResultValueParser<uint>  NewUIntResultParser => new();
    public static DefaultValueParser<uint>  NewUIntParser => new(0);
    public static ResultValueParser<ulong>  NewULongResultParser => new();
    public static DefaultValueParser<ulong>  NewULongParser => new(0);
    public static ResultValueParser<string>  NewStringResultParser => new();
    public static DefaultValueParser<string>  NewStringParser => new(string.Empty);

    public static CollectionValueParser<IList<T>, T> NewListValueParser<T>()
    {
        return new CollectionValueParser<IList<T>, T>(() => []);
    } 
    
    public static CollectionValueParser<ISet<T>, T> NewSetValueParser<T>()
    {
        return new CollectionValueParser<ISet<T>, T>(() => new HashSet<T>());
    } 
}
