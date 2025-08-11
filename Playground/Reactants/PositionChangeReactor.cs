using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Server.Api;
using Playground.Models;
using Playground.Models.Methods;

namespace Playground.Reactants;

public class PositionChangeReactor(
    IAgentSubscriptionManager<Gantry> subs,
    ICurrentAgentStateChannel<Gantry> currentAgentStateChannel,
    ILogger<PositionChangeReactor> logger,
    IGantryMethods gantryMethods,
    AgentServerStartedMarker marker)
    : BackgroundService
{
    private Position? LastPosition = null;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var values = Enum.GetValues(typeof(Position));
        var startPosition = (Position)values.GetValue(new Random().Next(values.Length))!;
        await marker;
        await subs.SubscribeToAllPropertyChanges(currentAgentStateChannel, stoppingToken);
        var reader = currentAgentStateChannel.Reader;
        await gantryMethods.ChangePosition(startPosition);
        while (await reader.WaitToReadAsync(stoppingToken))
        {
            var message = await reader.ReadAsync(stoppingToken);
            var newPosition = message.Payload.Position;

            if (LastPosition != newPosition)
            {
                logger.LogInformation("Position has changed, calculating next position");
                var next = newPosition switch
                {
                    Position.OverThere => Position.OverHere,
                    Position.OverHere => Position.OnTheMoon,
                    Position.OnTheMoon => Position.SanDiego,
                    Position.SanDiego => Position.Mexico,
                    Position.Mexico => Position.Aalborg,
                    Position.Aalborg => Position.TheSecretUndergroundLab,
                    Position.TheSecretUndergroundLab => Position.OverThere,
                    var _ => throw new ArgumentOutOfRangeException(newPosition + " is not handled.")
                };
                await gantryMethods.ChangePosition(next);
            }

            LastPosition = newPosition;
        }
    }
}