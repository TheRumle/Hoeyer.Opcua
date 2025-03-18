namespace Hoeyer.OpcUa.ClientTest;

[DependencyInjection]
public class CanCreateContainer(OpcUaServerContainer container)
{

    public async Task ContainerIsRunning()
    {
        await Assert.That(container.ServerContainer.Name).IsNotNullOrEmpty();
    }
    
}