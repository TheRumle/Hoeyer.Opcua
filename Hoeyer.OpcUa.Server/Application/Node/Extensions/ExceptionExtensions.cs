using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.Node.Extensions;

public static class ExceptionExtensions
{

    [Pure]
    public static IEnumerable<TException> CreateEntityViolations<T, TException>(this IEnumerable<T> values,
        IEntityNode entityNode,
        Func<T, NodeId> idSelector,
        Func<T, TException> exceptionSelector) where TException : Exception
    {
        return values
            .Where(e => idSelector(e).Equals(entityNode.Entity.NodeId))
            .Select(exceptionSelector.Invoke);
    }
}