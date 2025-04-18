﻿using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNode
{
    public BaseObjectState BaseObject { get; }
    public Dictionary<NodeId, PropertyState> PropertyStates { get; }
    public Dictionary<string, PropertyState> PropertyByBrowseName { get; }
}