using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public sealed record ConcretePartialMatcher<T>(ServiceLifetime Lifetime, Type? Implementation = null)
    : IPartialServiceMatcher
{
    public Type ServiceType { get; init; } = typeof(T);

    public bool Equals(ServiceDescriptor? other)
    {
        if (other is null)
        {
            return false;
        }

        var lifeTimeEquality = Lifetime == other.Lifetime;
        var implementationEquality = Implementation == null || Implementation == other.ImplementationType;
        return
            other.ServiceType == ServiceType
            && lifeTimeEquality
            && implementationEquality;
    }

    public override string ToString() => new TypeDescriptorString(this);
}