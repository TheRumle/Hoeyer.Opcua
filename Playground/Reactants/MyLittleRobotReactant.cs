using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Playground.Models;
using Playground.Models.Methods;

namespace Playground.Reactants;

public sealed class MyLittleRobotReactant(
    IEntitySubscriptionManager<MyLittleRobot> subManager,
    ICurrentEntityStateChannel<MyLittleRobot> stateChannel,
    ILittleRobotMethods methods)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        var subscription = await subManager.SubscribeToChange(stateChannel, token);
        var reader = stateChannel.Reader;
        while (await reader.WaitToReadAsync(token))
        {
            MyLittleRobot currentState = (await reader.ReadAsync(token)).Payload;
            if (currentState.Speed < 2.4f)
            {
                await methods.IncrementSpeed();
                subscription.Pause();
                return;
            }
        }
    }
}