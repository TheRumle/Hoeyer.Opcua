﻿using System.Collections.Generic;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityNode
{
    public BaseObjectState BaseObject { get; }
    public IEnumerable<PropertyState> PropertyStates { get; }
    public IEnumerable<MethodState> Methods { get; }
    public IReadOnlyDictionary<string, PropertyState> PropertyByBrowseName { get; }
    public IReadOnlyDictionary<string, MethodState> MethodsByName { get; }
}