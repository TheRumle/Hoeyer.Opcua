﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Hoeyer.Common.Extensions.Exceptions;

public static class ExceptionExtensions
{
    public static AggregateException ToAggregateException(this IEnumerable<Exception> exceptions)
    {
        return new AggregateException(exceptions);
    }

    public static TResult ToNewlineSeparated<TResult>(this IEnumerable<Exception> exceptions,
        Func<string, TResult> newException) where TResult : Exception
    {
        return ToCharSeparated(exceptions, '\n', newException);
    }

    public static TResult ToCharSeparated<TResult>(this IEnumerable<Exception> exceptions, char character,
        Func<string, TResult> newException) where TResult : Exception
    {
        return newException.Invoke(string.Join($"{character}", exceptions.Select(e => e.Message)));
    }

    public static TResult ToCharSeparated<TResult>(this IEnumerable<IError> exceptions, char character,
        Func<string, TResult> newException) where TResult : Exception
    {
        return newException.Invoke(string.Join($"{character}", exceptions.Select(e => e.Message)));
    }

    public static IError ToError(this Exception exception)
    {
        return new ExceptionalError(exception);
    }

    public static Exception ToArgumentException(this IEnumerable<IError> errors)
    {
        return new ArgumentException(errors.ToNewlineSeparatedString());
    }

    public static Exception ToInvalidOperationException(this IEnumerable<IError> errors)
    {
        return new ArgumentException(errors.ToNewlineSeparatedString());
    }
}