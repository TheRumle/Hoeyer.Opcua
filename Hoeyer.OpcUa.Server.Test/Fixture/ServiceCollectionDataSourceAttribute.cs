using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.IntegrationTest.Fixture;

[SuppressMessage(
    "Design", "S3993",
    Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public class ServiceCollectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata) =>
        ServiceProvider.CreateAsyncScope();

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type)!;

    private static IServiceProvider CreateSharedServiceProvider()
    {
        IServiceCollection services = new OpcUaServerServiceFixture().OnGoingOpcAgentServiceRegistration.Collection;
        services.AddSingleton(services);
        services.AddSingleton<IEnumerable<IMaybeInitializedAgentManager>>((p) =>
        {
            var collection = p.GetService<IServiceCollection>()!;
            IEnumerable<Type> wanted = collection
                .Where(e => e.ServiceType.IsAssignableTo(typeof(IMaybeInitializedAgentManager)))
                .Select(e => e.ImplementationType!);

            return wanted.Select(p.GetService).Select(value => (IMaybeInitializedAgentManager)value!);
        });

        services.AddSingleton<List<IMaybeInitializedAgentManager>>(p =>
            p.GetService<IEnumerable<IMaybeInitializedAgentManager>>()!.ToList());

        return services.BuildServiceProvider();
    }
}