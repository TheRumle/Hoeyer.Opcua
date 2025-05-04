namespace Hoeyer.OpcUa.Core.Api;


public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public bool AssignToNode(T state, IEntityNode node);
}