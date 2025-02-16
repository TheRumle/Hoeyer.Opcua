using System;
using System.Collections.Generic;

namespace Hoeyer.Common.Extensions.Exceptions;

public static class ExceptionExtensions
{
    public static AggregateException ToAggregateException(this IEnumerable<Exception> exceptions)
    {
        return new AggregateException(exceptions);
    }
    
}