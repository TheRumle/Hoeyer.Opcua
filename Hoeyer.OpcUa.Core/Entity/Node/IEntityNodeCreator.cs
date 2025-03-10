﻿using System.Diagnostics.Contracts;

namespace Hoeyer.OpcUa.Core.Entity.Node;

public interface IEntityNodeCreator<out T> : IEntityNodeCreator;

public interface IEntityNodeCreator
{
    public string EntityName { get; }

    [Pure]
    public EntityNode CreateEntityOpcUaNode(ushort assignedNamespace);
}