﻿using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Application.Translator.Parsers;

public static class DataValueParsers
{
    public static DefaultDataValueParser<int> NewIntDataValueParser => new(0);
    public static DefaultDataValueParser<float> NewFloatParser => new(0f);
    public static DefaultDataValueParser<double> NewDoubleParser => new(0.0);
    public static DefaultDataValueParser<decimal> NewDecimalParser => new(0m);
    public static DefaultDataValueParser<long> NewLongParser => new(0L);
    public static DefaultDataValueParser<short> NewShortParser => new(0);
    public static DefaultDataValueParser<byte> NewByteParser => new(0);
    public static DefaultDataValueParser<sbyte> NewSByteParser => new(0);
    public static DefaultDataValueParser<ushort> NewUShortParser => new(0);
    public static DefaultDataValueParser<uint> NewUIntParser => new(0);
    public static DefaultDataValueParser<ulong> NewULongParser => new(0);
    public static DefaultDataValueParser<string> NewStringParser => new(string.Empty);

    public static CollectionDataValueParser<IList<T>, T> NewListValueParser<T>() => new(() => []);

    public static CollectionDataValueParser<ISet<T>, T> NewSetValueParser<T>() => new(() => new HashSet<T>());
}