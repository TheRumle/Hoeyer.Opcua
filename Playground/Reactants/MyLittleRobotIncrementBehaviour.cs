using Hoeyer.OpcUa.Simulation.Api.Configuration;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;
using Playground.Models;
using Playground.Models.Methods.Generated;

namespace Playground.Reactants;

public sealed class MyLittleRobotIncrementBehaviour
    : ISimulation<MyLittleRobot, IncrementSpeedArgs> //marker for the increment
{
    public IEnumerable<ISimulationStep> ConfigureSimulation(
        ISimulationBuilder<MyLittleRobot, IncrementSpeedArgs>
            onMethodCall)
    {
        return onMethodCall
            .Wait(TimeSpan.FromMilliseconds(100))
            .ChangeState((context) =>
            {
                var robot = context.State;
                robot.Speed += 0.4f;
            })
            .Build();
    }
}