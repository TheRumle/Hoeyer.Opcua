using System.Threading.Channels;
using Hoeyer.Common.Extensions.Types;
using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Services;
using Hoeyer.OpcUa.EntityModelling.Methods;
using Hoeyer.OpcUa.EntityModelling.Models;
using Hoeyer.OpcUa.Server.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.EntityModelling.Reactants;

public class PositionChangeReactor(
    IStateChangeObserver<Gantry> observer,
    ILogger<PositionChangeReactor> logger,
    IGantryMethods gantryMethods,
    IEntityServerStartedMarker marker)
    : BackgroundService
{
    private Position? LastPosition = null;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var values = Enum.GetValues(typeof(Position));
        var startPosition = (Position)values.GetValue(new Random().Next(values.Length))!;
        await marker;
        var (channel, subscription) = await observer
            .BeginObserveAsync()
            .ThenAsync(e => (e.StateChangeChannel, e.Subscription));
        await gantryMethods.ChangePosition(startPosition); //initialize the whole sha-bang by changing a position

        await ReactToPositionChanges(stoppingToken, channel);
    }

    private async Task ReactToPositionChanges(CancellationToken stoppingToken, ChannelReader<IMessage<Gantry>> reader)
    {
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