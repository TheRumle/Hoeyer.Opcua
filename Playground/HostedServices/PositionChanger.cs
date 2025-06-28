using System.Threading.Channels;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Monitoring;
using Hoeyer.OpcUa.Client.Application.Subscriptions;
using Hoeyer.OpcUa.Server.Api;
using Playground.Models;
using Playground.Models.Methods;

namespace Playground.HostedServices;

public class PositionChanger(EntityServerStartedMarker marker, IGantryMethods methods, ILogger<PositionChanger> logger)
    : BackgroundService
{
    public async Task ChangePosition(Position p)
    {
        await marker.ServerRunning();
        logger.LogInformation("Attempting to change position to " + p);
        await methods.ChangePosition(p);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
}

public class PositionChangeReactor(
    IEntitySubscriptionManager<Gantry> subs,
    ICurrentEntityStateChannel<Gantry> channel,
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
        var subscription = await subs.SubscribeToChange(channel, stoppingToken);
        await gantryMethods.ChangePosition(startPosition);
        var reader = channel.Reader;
        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        while (await reader.WaitToReadAsync(stoppingToken))
        {
            var message = await reader.ReadAsync(stoppingToken);
            var newPosition = message.Payload.Position;
            if (LastPosition is null)
            {
                LastPosition = newPosition;
                continue;
            }

            if (LastPosition != newPosition)
            {
                logger.LogInformation("Position has changed, calculating next position");
                var next = newPosition switch
                {
                    Position.OverThere => Position.OverHere,
                    Position.OverHere => Position.OnTheMoon,
                    Position.OnTheMoon => Position.OverThere,
                    var _ => throw new ArgumentOutOfRangeException()
                };
                await gantryMethods.ChangePosition(next);
            }

            LastPosition = newPosition;
        }
    }
}