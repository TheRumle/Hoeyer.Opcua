﻿namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface INodeHolder<T>
{
    public void TakeSharedNode(IManagedEntityNode<T> managedNode);
}