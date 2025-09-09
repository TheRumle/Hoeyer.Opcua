using System;

namespace Hoeyer.Common.Utilities.Threading;

public interface ILocked<out T>
{
    /// <summary>
    ///     Locks the state and mutates it using the given action
    /// </summary>
    public void ChangeState(Action<T> stateChanges);

    /// <summary>
    ///     Lock the state and compute an expression over it
    /// </summary>
    /// <param name="computation">The computation to perform over the node</param>
    /// <typeparam name="TOut">The type of the selected value</typeparam>
    /// <returns>The selected value</returns>
    public TOut Select<TOut>(Func<T, TOut> computation);

    /// <summary>
    ///     Lock the state and examine it and execute side-effects based on it
    /// </summary>
    /// <param name="effect">The computation to perform over the statet</param>
    public void Examine(Action<T> effect);
}