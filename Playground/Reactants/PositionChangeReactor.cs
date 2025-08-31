using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.TestEntities.Methods;
using Hoeyer.OpcUa.TestEntities.Models;

namespace Playground.Reactants;

public class PositionChangeReactor(
    IEntitySubscriptionManager<Gantry> subs,
    ICurrentEntityStateChannel<Gantry> currentEntityStateChannel,
    ILogger<PositionChangeReactor> logger,
    IGantryMethods gantryMethods,
    EntityServerStartedMarker marker)
    : BackgroundService
{
    private Position? LastPosition = null;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var values = Enum.GetValues(typeof(Position));
        var startPosition = (Position)values.GetValue(new Random().Next(values.Length))!;
        await marker;
        await subs.SubscribeToProperty(currentEntityStateChannel, nameof(Gantry.Position), stoppingToken);
        await gantryMethods.ChangePosition(startPosition); //initialize the whole sha-bang by changing a position
        await ReactToPositionChanges(stoppingToken);
    }

    private async Task ReactToPositionChanges(CancellationToken stoppingToken)
    {
        var reader = currentEntityStateChannel.Reader;
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
                    Position.Mexico => Position.Submarine,
                    Position.Submarine => Position.TheSecretUndergroundLab,
                    Position.TheSecretUndergroundLab => Position.OverThere,
                    var _ => throw new ArgumentOutOfRangeException(newPosition + " is not handled.")
                };
                await gantryMethods.ChangePosition(next);
            }

            LastPosition = newPosition;
        }
    }
}