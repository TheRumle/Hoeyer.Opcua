using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hoeyer.OpcUa.Client.Application;
using Hoeyer.OpcUa.Client.Application.MachineProxy;
using Hoeyer.OpcUa.Client.Configuration.Entities.Builder;
using Hoeyer.OpcUa.Client.Configuration.Entities.Exceptions;
using Hoeyer.OpcUa.Client.Reflection.Configurations;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity.State;
using Hoeyer.OpcUa.Observation;
using Hoeyer.OpcUa.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hoeyer.OpcUa.Client.Services;

/// <summary>
///     Uses the <paramref name="services" /> to create a <see cref="ServiceProvider" /> and uses it to wire up services
///     necessary for configuring OpcUa entities. Disposing the <see cref="ServiceRegistry" /> will remove services used
///     for internal wiring from <paramref name="services" />.
/// </summary>
/// <param name="services">
///     The collection providing the register with services necessary for the creation of a number of
///     services related to OpcUa state observation
/// </param>
internal sealed class ServiceRegistry(IServiceCollection services)
{
   
}