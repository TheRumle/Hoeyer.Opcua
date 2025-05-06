using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Server.Api;

namespace MyOpcUaWebApplication;

public class ReaderHost(IEntityBrowser<Gantry> client, IEntityWriter<Gantry> writer, EntityServerStartedMarker marker) : BackgroundService
{
    private readonly Random _random = new();
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await marker.ServerRunning();
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await client.BrowseEntityNode(stoppingToken);

            foreach (var (propertyName, propertyState) in result.PropertyByBrowseName)
            {
                Console.WriteLine(propertyName + " has the value " + propertyState.Value);
            }

            Console.WriteLine("___________________________________________________");
            Console.WriteLine();
            Console.WriteLine("___________________________________________________");

            await writer.AssignEntityValues(CreateRandomGantry(), stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
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
