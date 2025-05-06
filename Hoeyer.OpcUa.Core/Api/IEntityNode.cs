using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityNode
{
    public BaseObjectState BaseObject { get; }
    public ISet<PropertyState> PropertyStates { get; }
    public ISet<MethodState> Methods { get; }
    public Dictionary<string, PropertyState> PropertyByBrowseName { get; }
}