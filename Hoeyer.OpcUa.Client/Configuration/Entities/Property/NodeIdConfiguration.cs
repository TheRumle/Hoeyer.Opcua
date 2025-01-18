using System;
using Hoeyer.OpcUa.Client.Configuration.Entities.Builder;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Configuration.Entities.Property;

/// <summary>
///     Represents a valid GetNodeId string. Used to ensure that id's are given in the correct string format.
///     Valid GetNodeId strings are of the form: "i=1234", "s=HelloWorld", "g=AF469096-F02A-4563-940B-603958363B81",
///     "b=01020304", "ns=2;s=HelloWorld", "ns=2;i=1234", "ns=2;g=AF469096-F02A-4563-940B-603958363B81", "ns=2;b=01020304"
/// </summary>
public class NodeIdConfiguration
{
    public readonly string IdString;

    public NodeIdConfiguration(string name)
    {
        IdString = $"s={name}";
    }

    /// Constructor for numeric identifier (e.g., "i=1234").
    public NodeIdConfiguration(int i)
    {
        IdString = "i=" + i;
    }


    /// Constructor for GUID identifier (e.g., "g=AF469096-F02A-4563-940B-603958363B81").
    public NodeIdConfiguration(Guid g)
    {
        IdString = "g=" + g;
    }

    public NodeId ToNodeId()
    {
        return NodeId.Parse(IdString);
    }

    public NodeId ToNodeId(RootIdentity identity)
    {
        return NodeId.Parse("ns=2;s=mainGantry");
    }
}