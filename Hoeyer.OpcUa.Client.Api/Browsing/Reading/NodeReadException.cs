using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Common.Extensions;
using Opc.Ua;
using static Hoeyer.OpcUa.Client.Api.StatusCodeExtensions;

namespace Hoeyer.OpcUa.Client.Api.Browsing.Reading;

public sealed class NodeReadException : Exception
{
    public NodeReadException(NodeId toRead, ServiceResultException message) : this([toRead], message)
    {
    }

    public NodeReadException(NodeId toRead, string message) : base(
        $"Failed reading {toRead.ToString()} due to error: {message}")
    {
    }

    public NodeReadException(IEnumerable<NodeId> toRead, string message) : base(
        $"Failed reading {toRead.ToCommaSeparatedString()} due to error: {message}")
    {
    }

    public NodeReadException(IEnumerable<NodeId> toRead, ServiceResultException cause)
        : this(
            $"Failed reading [{toRead.ToCommaSeparatedString()}] due to failure '{GetStatusCodeName(cause.StatusCode)}' with cause " +
            cause.Message + $" and payload: {cause.Data}. See {cause.HelpLink}")
    {
    }

    private NodeReadException(string message) : base(message)
    {
    }

    public NodeReadException(IEnumerable<NodeId> toRead, IList<ServiceResultException> cause)
        : this(
            $"Failed reading [{toRead.ToCommaSeparatedString()}] due to failures '{cause.Select(e => GetStatusCodeName(e.StatusCode)).SeparateBy(", ")}' with reasons {cause.Select((e, index) => (e.Message, index)).Aggregate("\n", (current, elem) => current + elem.index + ". " + elem.Message + ", ")}. \n " +
            $"The payloads were: {cause.Select((e, index) => (e, index)).Aggregate("\n", (current, elem) => current + elem.index + ". " + $"{elem.e.Data}. See {elem.e.HelpLink}")}")
    {
    }
}