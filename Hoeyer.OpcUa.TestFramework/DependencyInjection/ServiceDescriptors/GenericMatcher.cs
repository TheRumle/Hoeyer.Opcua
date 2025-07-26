using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public sealed record GenericMatcher : IPartialServiceMatcher
{
    public GenericMatcher(Type ServiceType, ServiceLifetime Lifetime, Type? Implementation = null)
    {
        this.ServiceType = ServiceType;
        this.Lifetime = Lifetime;
        this.Implementation = Implementation;

        if (!ServiceType.IsGenericTypeDefinition)
        {
            throw new ArgumentException($"{nameof(ServiceType)} must be a generic type definition");
        }
    }

    public bool Equals(ServiceDescriptor? other)
    {
        if (other is null)
        {
            return false;
        }

        var lifeTimeEquality = Lifetime == other.Lifetime;
        var implementationEquality = Implementation == null || Implementation == other.ImplementationType;
        var serviceEquality = ServiceType == other.ServiceType ||
                              (other.ServiceType.IsConstructedGenericType &&
                               ServiceType == other.ServiceType.GetGenericTypeDefinition());
        return
            serviceEquality
            && lifeTimeEquality
            && implementationEquality;
    }

    public Type ServiceType { get; init; }
    public ServiceLifetime Lifetime { get; init; }
    public Type? Implementation { get; init; }
    public override string ToString() => new TypeDescriptorString(this);
}