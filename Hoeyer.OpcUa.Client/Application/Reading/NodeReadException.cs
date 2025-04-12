using System;
using System.Collections.Generic;
using Hoeyer.Common.Extensions;
using Opc.Ua;
using static Hoeyer.OpcUa.Core.Extensions.Opc.StatusCodeExtensions;

namespace Hoeyer.OpcUa.Client.Application.Reading;

public sealed class NodeReadException : Exception
{

    public NodeReadException(NodeId toRead, ServiceResultException message) : this([toRead], message)
    {
    }
    
    public NodeReadException(NodeId toRead, string message) : base($"Failed reading {toRead.ToString()} due to error: {message}") 
    {
    }
    
    public NodeReadException(IEnumerable<NodeId> toRead, string message) : base($"Failed reading {toRead.ToCommaSeparatedString()} due to error: {message}") 
    {
    }

    public NodeReadException(IEnumerable<NodeId> toRead, ServiceResultException cause) 
        : this($"Failed reading [{toRead.ToCommaSeparatedString()}] due to failure '{GetStatusCodeName(cause.StatusCode)}' with cause " + cause.Message + $" and payload: {cause.Data}. See {cause.HelpLink}")
    {}

    private NodeReadException(string message) : base(message)
    {
    }
}