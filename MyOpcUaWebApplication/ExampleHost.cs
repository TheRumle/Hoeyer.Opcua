using Hoeyer.Common.Messaging.Api;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Server.Api;
using Opc.Ua;

namespace MyOpcUaWebApplication;

public class ExampleHost(
    IEntityBrowser<Gantry> client,
    IEntityWriter<Gantry> writer,
    EntityServerStartedMarker marker,
    IGantryMethods methods) : BackgroundService
{
    private readonly Random _random = new();

    public IMessageSubscription Subscription { get; set; }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        while (!stoppingToken.IsCancellationRequested)
        {
            IEntityNode result = await client.BrowseEntityNode(stoppingToken);
            foreach ((var propertyName, PropertyState propertyState) in result.PropertyByBrowseName)
            {
                Console.WriteLine(propertyName + " has the value " + propertyState.Value);
            }

            Console.WriteLine("___________________________________________________");
            Console.WriteLine("___________________________________________________");

            await writer.AssignEntityValues(CreateRandomGantry(), stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            Console.WriteLine("Calling the method Position resulted in " + await methods.Position("q", 2f, []));
            await writer.AssignEntityValues(CreateRandomGantry(), stoppingToken);
        }
    }

    private Gantry CreateRandomGantry()
    {
        return new Gantry()
        {
            message = "this is tomfoolery",
            messages = ["But it also works", "As a great example"],
            Names = ["to show how things work", "because moving will fluctuate between true and false randomly"],
            Moving = _random.Next() % 2 == 0,
            Position = _random.Next() % 2 == 0 ? Position.OnTheMoon : Position.OverHere,
            Speeds = Enumerable.Range(0, 10).Select(_ => _random.Next(9)).ToList()
        };
    }
}