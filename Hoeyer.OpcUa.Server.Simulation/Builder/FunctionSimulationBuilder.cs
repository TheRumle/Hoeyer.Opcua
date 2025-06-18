using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;

namespace Hoeyer.OpcUa.Server.Simulation.Builder;

internal sealed class CommonSimulationOperations<TEntity, TArguments>(
    IManagedEntityNode currentState,
    Queue<ISimulationStep> queue,
    ISimulationStepFactory<TEntity, TArguments> stepFactory)
{
    public void ChangeState(Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        ActionStep<TEntity, TArguments> step = stepFactory.CreateActionStep(currentState, stateChange);
        queue.Enqueue(step);
    }

    public void ChangeStateAsync(Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        AsyncActionStep<TEntity, TArguments> action = stepFactory.CreateAsyncActionStep(currentState, stateChange);
        queue.Enqueue(action);
    }

    public void SideEffect(Action<TArguments> sideEffect)
    {
        queue.Enqueue(new SideEffectActionStep<TArguments>(sideEffect));
    }

    public void Wait(TimeSpan timeSpan)
    {
        TimeStep<TEntity> step = stepFactory.CreateTimeStep(currentState, timeSpan);
        queue.Enqueue(step);
    }
}

internal sealed class
    FunctionSimulationBuilder<TEntity, TArguments, TReturn> : IFunctionSimulationBuilder<TEntity, TArguments, TReturn>
{
    private readonly CommonSimulationOperations<TEntity, TArguments> _commonOperations;
    private readonly IManagedEntityNode _node;
    private readonly Queue<ISimulationStep> _simulationSteps;
    private readonly ISimulationStepFactory<TEntity, TArguments> _stepFactory;

    public FunctionSimulationBuilder(IManagedEntityNode node,
        ISimulationStepFactory<TEntity, TArguments> stepFactory)
    {
        _node = node;
        _stepFactory = stepFactory;
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new CommonSimulationOperations<TEntity, TArguments>(_node, _simulationSteps, _stepFactory);
    }


    /// <inheritdoc />
    public IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TEntity, TArguments>, TReturn> returnValueFactory)
    {
        ReturnValueStep<TEntity, TArguments, TReturn> step =
            _stepFactory.CreateReturnValueStep(_node, returnValueFactory);
        _simulationSteps.Enqueue(step);
        return _simulationSteps.ToArray();
    }


    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TEntity, TArguments>> stateChange)
    {
        _commonOperations.ChangeState(stateChange);
        return this;
    }

    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> ChangeStateAsync(
        Func<SimulationStepContext<TEntity, TArguments>, ValueTask> stateChange)
    {
        _commonOperations.ChangeStateAsync(stateChange);
        return this;
    }

    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> SideEffect(Action<TArguments> sideEffect)
    {
        _commonOperations.SideEffect(sideEffect);
        return this;
    }

    public IFunctionSimulationBuilder<TEntity, TArguments, TReturn> Wait(TimeSpan timeSpan)
    {
        _commonOperations.Wait(timeSpan);
        return this;
    }
}