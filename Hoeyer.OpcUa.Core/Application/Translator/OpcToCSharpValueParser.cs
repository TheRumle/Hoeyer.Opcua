using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.Translator;

public static class OpcToCSharpValueParser
{
    public static object ParseOpcValue(object value)
    {
        return value switch
        {
            Uuid uuid => Guid.Parse(uuid.GuidString),
            var _ => value
        };
    }
}