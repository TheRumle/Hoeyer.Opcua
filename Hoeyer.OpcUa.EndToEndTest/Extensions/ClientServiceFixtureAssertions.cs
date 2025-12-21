using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;

namespace Hoeyer.OpcUa.EndToEndTest.Extensions;

public static class ClientServiceFixtureAssertions
{
    public static async Task AssertInteractions<T>(this SimulationFixture<T> fixture,
        Func<SimulationContext<T>, Task> assertionProvider)
    {
        using var assertion = Assert.Multiple();
        var servicesUnderTest = fixture.GetServicesUnderTest();
        foreach (var serviceUnderTest in servicesUnderTest)
        {
            await assertionProvider.Invoke(serviceUnderTest);
        }
    }

    public static async Task AssertInteractions<T>(this SimulationFixture<T> fixture,
        Action<SimulationContext<T>> assertionProvider)
    {
        using var assertion = Assert.Multiple();
        var servicesUnderTest = fixture.GetServicesUnderTest();

        await Task.WhenAll(servicesUnderTest.Select(serviceContext =>
            Task.Run(() => assertionProvider.Invoke(serviceContext))));
    }


    public static async Task AssertAll<T>(this SimulationFixture<T> fixture,
        Func<T, Task> assertionProvider)
    {
        using var assertion = Assert.Multiple();
        var servicesUnderTest = fixture.GetServicesUnderTest();
        foreach (var serviceUnderTest in servicesUnderTest)
        {
            await assertionProvider.Invoke(serviceUnderTest.TestedService);
        }

        foreach (var serviceUnderTest in servicesUnderTest)
        {
            await assertionProvider.Invoke(serviceUnderTest.TestedService);
        }
    }
}