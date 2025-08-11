using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Configuration;

internal sealed class SimulationBuilder<TAgent, TArguments> : ISimulationBuilder<TAgent, TArguments>
{
    private readonly
        CompositeActionSimulationBuilder<TAgent, TArguments, SimulationBuilder<TAgent, TArguments>>
        _commonOperations;

    private readonly Queue<ISimulationStep> _simulationSteps;

    public SimulationBuilder(IAgentTranslator<TAgent> translator)
    {
        _simulationSteps = new Queue<ISimulationStep>();
        _commonOperations = new(this, translator, _simulationSteps);
    }

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToArray();

    public ISimulationBuilder<TAgent, TArguments> ChangeState(
        Action<SimulationStepContext<TAgent, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    public ISimulationBuilder<TAgent, TArguments> ChangeStateAsync(
        Func<SimulationStepContext<TAgent, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    public ISimulationBuilder<TAgent, TArguments> SideEffect(
        Action<SimulationStepContext<TAgent, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    public ISimulationBuilder<TAgent, TArguments> Wait(TimeSpan timeSpan) => _commonOperations.Wait(timeSpan);
}

internal sealed class
    SimulationBuilder<TAgent, TArguments, TReturn> : ISimulationBuilder<TAgent, TArguments, TReturn>
{
    private readonly CompositeActionSimulationBuilder<TAgent, TArguments,
            ISimulationBuilder<TAgent, TArguments, TReturn>>
        _commonOperations;

    private readonly Queue<ISimulationStep> _simulationSteps = new();
    private readonly IAgentTranslator<TAgent> _translator;

    public SimulationBuilder(IAgentTranslator<TAgent> translator)
    {
        this._translator = translator;
        _commonOperations = new(this, translator, _simulationSteps);
    }


    public IEnumerable<ISimulationStep> WithReturnValue(
        Func<SimulationStepContext<TAgent, TArguments>, TReturn> returnValueFactory)
    {
        _simulationSteps.Enqueue(
            new ReturnValueStep<TAgent, TArguments, TReturn>(returnValueFactory, _translator.Copy));
        return _simulationSteps.ToArray();
    }

    public ISimulationBuilder<TAgent, TArguments, TReturn> ChangeState(
        Action<SimulationStepContext<TAgent, TArguments>> stateChange) => _commonOperations.ChangeState(stateChange);

    public ISimulationBuilder<TAgent, TArguments, TReturn> ChangeStateAsync(
        Func<SimulationStepContext<TAgent, TArguments>, ValueTask> stateChange) =>
        _commonOperations.ChangeStateAsync(stateChange);

    public ISimulationBuilder<TAgent, TArguments, TReturn> SideEffect(
        Action<SimulationStepContext<TAgent, TArguments>> sideEffect) => _commonOperations.SideEffect(sideEffect);

    public ISimulationBuilder<TAgent, TArguments, TReturn> Wait(TimeSpan timeSpan) =>
        _commonOperations.Wait(timeSpan);

    public IEnumerable<ISimulationStep> Build() => _simulationSteps.ToList();
}