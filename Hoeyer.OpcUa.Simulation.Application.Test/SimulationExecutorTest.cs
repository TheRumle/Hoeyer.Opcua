using Hoeyer.Common.Extensions.Async;
using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Hoeyer.OpcUa.Simulation.Api.PostProcessing;
using Hoeyer.OpcUa.TestEntities;
using Hoeyer.OpcUa.TestEntities.Methods;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;
using Simulation.Application.Test.Fixtures;

namespace Simulation.Application.Test;

[ServiceInjectionDataGenerator]
public sealed class SimulationExecutorTest(
    ISimulationExecutor<Gantry, GetCurrentContainerIdArgs> executor,
    ISimulationBuilder<Gantry, GetCurrentContainerIdArgs> simulationBuilder)
{
    [Test]
    public async Task WhenChangingState_NTimes_NStateChangeActionsAreReturned()
    {
        var state = new Gantry
        {
            AAginList = new List<string>(),
            Position = Position.OnTheMoon,
            AList = [],
            HeldContainer = Guid.CreateVersion7(),
            IntValue = Random.Shared.Next(),
            Occupied = true,
            StringValue = "Hello"
        };

        var resultingActionTypes = (await ExecuteUnderState(state)).Select(e => e.Action).ToList();
        await Assert.That(resultingActionTypes).All().Satisfy(x => x.IsEqualTo(ActionType.StateMutation));
        await Assert.That(resultingActionTypes.Count).IsEqualTo(Enum.GetValues<Position>().Length);
    }

    private async Task<IEnumerable<SimulationResult<Gantry>>> ExecuteUnderState(Gantry init)
    {
        return await CreateAndExecute(init, new GetCurrentContainerIdArgs(),
            builder =>
            {
                foreach (var pos in Enum.GetValues<Position>())
                {
                    builder.ChangeState(e => e.State.Position = pos);
                }

                return builder.Build();
            });
    }

    private async Task<IEnumerable<SimulationResult<Gantry>>> CreateAndExecute(Gantry init,
        GetCurrentContainerIdArgs args,
        Func<ISimulationBuilder<Gantry, GetCurrentContainerIdArgs>, IEnumerable<ISimulationStep>> builder)
    {
        var steps = builder.Invoke(simulationBuilder);
        return await executor.ExecuteSimulation(init, args, steps).Collect();
    }
}