namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public void AssignToNode(T state, IEntityNode node);
}