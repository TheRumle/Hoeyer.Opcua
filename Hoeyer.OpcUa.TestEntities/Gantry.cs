using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Simulation.Api;
using Hoeyer.OpcUa.Server.Simulation.Services.Action;
using Hoeyer.OpcUa.Server.Simulation.Services.SimulationSteps;
using Hoeyer.opcUa.TestEntities.Methods.Generated;

namespace Hoeyer.opcUa.TestEntities;

[OpcUaEntity]
public sealed class Gantry
{
    public int IntValue { get; set; }
    public string StringValue { get; set; }
    public List<string> AList { get; set; }
    public List<string> AAginList { get; set; }
}

public sealed class ChangeGantryPositionActionSimulation : IActionSimulationConfigurator<Gantry, IntegerInputArgs>
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        IActionSimulationBuilder<Gantry, IntegerInputArgs> actionSimulationConfiguration) =>
        MakeSimulation(actionSimulationConfiguration);

    private static IEnumerable<ISimulationStep> MakeSimulation(
        IActionSimulationBuilder<Gantry, IntegerInputArgs> actionSimulationConfiguration)
    {
        return actionSimulationConfiguration
            .ChangeState(currentState => { currentState.State.IntValue = new Random(2).Next(); })
            .Wait(TimeSpan.FromSeconds(3))
            .ChangeState(currentState =>
            {
                currentState.State.StringValue = (currentState.Arguments.Q + currentState.Arguments.Q).ToString();
            })
            .Build();
    }
}