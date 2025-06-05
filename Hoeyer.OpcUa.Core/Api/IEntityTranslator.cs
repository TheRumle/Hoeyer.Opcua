using System;

namespace Hoeyer.OpcUa.Core.Api;

public interface IEntityTranslator<T>
{
    public T Translate(IEntityNode state);
    public void AssignToNode(T state, IEntityNode node);

    /// <summary>
    /// Providing a property name and its value and invokes <see cref="assignment"/> using it.
    /// </summary>
    /// <param name="state">The entity used for the assignment</param>
    /// <param name="assignment">An assignment based on name and value</param>
    public void AssignToStructure(T state, Action<string, object> assignment);
}