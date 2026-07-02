namespace Hoeyer.OpcUa.Server.Abstractions.NodeManagement;

public interface INodeHolder<T>
{
    public void TakeSharedNode(IManagedEntityNode<T> managedNode);
}