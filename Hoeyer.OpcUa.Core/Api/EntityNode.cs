using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public sealed record EntityNode(BaseObjectState BaseObject, ISet<PropertyState> PropertyStates, ISet<MethodState> Methods)
    : IEntityNode
{
    public BaseObjectState BaseObject { get; } = BaseObject;

    public ISet<PropertyState> PropertyStates { get; } = PropertyStates;

    /// <inheritdoc />
    public ISet<MethodState> Methods { get; } = Methods;

    public Dictionary<string, PropertyState> PropertyByBrowseName => PropertyStates.ToDictionary(e => e.BrowseName.Name);

}