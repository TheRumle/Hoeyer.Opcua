using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Client.Api.Calling.Exceptions;

public sealed class NoSuchEntityMethodException : Exception
{
    public NoSuchEntityMethodException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NoSuchEntityMethodException(string entity, string methodName) : this(
        $"The entity {entity} does not have CallMethod method called {methodName}")
    {
    }

    public NoSuchEntityMethodException(string message) : base(message)
    {
    }

    public static NoSuchEntityMethodException NotImplementedOnServer(string methodName,
        Exception? innerException, params IEnumerable<object> args)
    {
        var pars = string.Join(",", args);
        var message =
            $"Server responded with BadNotImplemented. The method '{methodName}' with input params '{pars}' is not implemented on the server.";

        return innerException == null
            ? new NoSuchEntityMethodException(message)
            : new NoSuchEntityMethodException(message, innerException);
    }
}