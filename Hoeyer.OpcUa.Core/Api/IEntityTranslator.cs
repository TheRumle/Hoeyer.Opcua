using System;

namespace Hoeyer.OpcUa.Core.Api;

public interface IAgentTranslator<T>
{
    public T Translate(IAgent state);
    public void AssignToNode(T state, IAgent node);

    /// <summary>
    /// Providing a property name and its value and invokes <see cref="assignment"/> using it.
    /// </summary>
    /// <param name="state">The agent used for the assignment</param>
    /// <param name="assignment">An assignment based on name and value</param>
    public void AssignToStructure(T state, Action<string, object> assignment);

    public T Copy(T state);
}