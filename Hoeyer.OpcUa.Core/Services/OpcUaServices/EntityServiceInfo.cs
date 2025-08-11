using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

internal sealed record AgentServiceInfo
{
    /// <summary>
    /// Represents a Service of type IMyService &lt;T&gt; where T remains generic.
    /// </summary>
    public AgentServiceInfo(
        Type serviceType,
        Type ImplementationType,
        Type Agent,
        ServiceLifetime lifetime
    )
    {
        ServiceLifetime = lifetime;
        ServiceType = serviceType;
        this.ImplementationType = ImplementationType;
        this.Agent = Agent;

        if (Agent.GetCustomAttribute<OpcUaAgentAttribute>() is null)
            throw new ArgumentException(
                $"The type '{ImplementationType.Name}' is being registered as '{serviceType.Name}' but '{Agent.Name}' must be annotated with {nameof(OpcUaAgentAttribute)} to be considered an Agent.");
    }

    public ServiceLifetime ServiceLifetime { get; }

    public Type ServiceType { get; }
    public Type ImplementationType { get; }

    public Type Agent { get; }

    public override string ToString() => $"{ServiceType.FullName} being implemented by {ImplementationType.FullName}";

    public IServiceCollection AddToCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.Add(new ServiceDescriptor(ServiceType, ImplementationType, ServiceLifetime));
        serviceCollection.Add(new ServiceDescriptor(ImplementationType, ImplementationType, ServiceLifetime));
        return serviceCollection;
    }
}