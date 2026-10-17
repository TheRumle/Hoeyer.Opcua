namespace Hoeyer.OpcUa.Core.Abstractions;

public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public void AssignToNode(T state, IEntityNode node);

    /// <summary>
    ///     Providing a property name and its value and invokes <see cref="structure" /> using it.
    /// </summary>
    /// <param name="state">The entity used for the assignment</param>
    /// <param name="structure">A structure to assign the entity state into</param>
    public void AssignToStructure(T state, EntityNodeStructure structure);

    public T Copy(T state);
}