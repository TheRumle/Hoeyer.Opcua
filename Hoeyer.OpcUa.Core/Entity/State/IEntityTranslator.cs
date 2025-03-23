using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Core.Entity.State;

public interface IEntityTranslator;

public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public bool AssignToNode(T state, IEntityNode node);
}