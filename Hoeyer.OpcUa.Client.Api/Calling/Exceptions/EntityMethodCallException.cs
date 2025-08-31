using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Client.Api.Calling.Exceptions;

public sealed class EntityMethodCallException : Exception
{
    public EntityMethodCallException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public EntityMethodCallException(string entity, string methodName) : this(
        $"The entity {entity} does not have CallMethod method called {methodName}")
    {
    }

    public EntityMethodCallException(string message) : base(message)
    {
    }

    public static EntityMethodCallException NotImplementedOnServer(string methodName,
        Exception? innerException, params IEnumerable<object> args)
    {
        var pars = string.Join(",", args);
        var message =
            $"Server responded with BadNotImplemented. The method '{methodName}' with input params [{pars}] is not implemented on the server.";

        return innerException == null
            ? new EntityMethodCallException(message)
            : new EntityMethodCallException(message, innerException);
    }

    public static EntityMethodCallException InternalServerError(string methodName,
        Exception? innerException, params IEnumerable<object> args)
    {
        var pars = string.Join(",", args);
        var message =
            $"An internal server error occured while trying to call '{methodName}' with input params [{pars}].";

        return innerException == null
            ? new EntityMethodCallException(message)
            : new EntityMethodCallException(message, innerException);
    }
}