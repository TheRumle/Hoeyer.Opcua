using System;
using Hoeyer.Common.Utilities.Threading;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Simulation.ServerAdapter;

public sealed class LockedEntityState<TState>(
    ILocked<IEntityNode> node,
    IEntityTranslator<TState> translator) : ILocked<TState>
{
    private readonly object _lock = new();

    public void Examine(Action<TState> effect)
    {
        node.Examine(e => effect(translator.Translate(e)));
    }

    public void ChangeState(Action<TState> stateChanges)
    {
        lock (_lock)
        {
            node.ChangeState(e =>
            {
                var state = translator.Translate(e);
                stateChanges(state);
                translator.AssignToNode(state, e);
            });
        }
    }

    public TOut Select<TOut>(Func<TState, TOut> computation)
    {
        lock (_lock)
        {
            return node.Select(e => computation(translator.Translate(e)));
        }
    }
}