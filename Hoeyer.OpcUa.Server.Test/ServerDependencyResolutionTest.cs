using Hoeyer.OpcUa.Fixtures.Common.Test;
using Hoeyer.OpcUa.Server.Test.Fixtures.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Test;

[InheritsTests]
public sealed class ServerDependencyResolutionTest() : DependencyResolutionTest(
    ServiceInjectionAttribute.Services.ServiceProvider.GetRequiredService<IServiceCollection>(), typeof(AlarmTestEntity));