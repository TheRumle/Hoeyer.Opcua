using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Core.Entity;


public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public bool AssignToNode(T state, IEntityNode node);
}