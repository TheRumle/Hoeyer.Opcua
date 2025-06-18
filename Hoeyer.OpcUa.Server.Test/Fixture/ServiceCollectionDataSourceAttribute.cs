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

    public override object Create(IServiceScope scope, Type type) => scope.ServiceProvider.GetService(type);

    private static IServiceProvider CreateSharedServiceProvider()
    {
        IServiceCollection services = new OpcUaServerServiceFixture().OnGoingOpcEntityServiceRegistration.Collection;
        services.AddSingleton(services);
        services.AddSingleton<IEnumerable<IMaybeInitializedEntityManager>>((p) =>
        {
            var collection = p.GetService<IServiceCollection>()!;
            IEnumerable<Type> wanted = collection
                .Where(e => e.ServiceType.IsAssignableTo(typeof(IMaybeInitializedEntityManager)))
                .Select(e => e.ImplementationType!);

            return wanted.Select(p.GetService).Select(value => (IMaybeInitializedEntityManager)value);
        });

        services.AddSingleton<List<IMaybeInitializedEntityManager>>(p =>
            p.GetService<IEnumerable<IMaybeInitializedEntityManager>>()!.ToList());

        return services.BuildServiceProvider();
    }
}