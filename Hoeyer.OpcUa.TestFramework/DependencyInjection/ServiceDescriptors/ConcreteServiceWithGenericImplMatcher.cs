using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public sealed record ConcreteServiceWithGenericImplMatcher<T> : IPartialServiceMatcher
{
    public ConcreteServiceWithGenericImplMatcher(Type Implementation, ServiceLifetime Lifetime)
    {
        this.Lifetime = Lifetime;
        this.Implementation = Implementation;
        if (!Implementation.IsGenericTypeDefinition)
        {
            throw new ArgumentException(@"Implementation must be a generic type definition", nameof(Implementation));
        }
    }

    public Type ServiceType { get; init; } = typeof(T);
    public ServiceLifetime Lifetime { get; init; }
    public Type? Implementation { get; init; }

    public bool Equals(ServiceDescriptor? other)
    {
        if (other is null)
        {
            return false;
        }

        var lifeTimeEquality = Lifetime == other.Lifetime;
        var implementationEquality = Implementation == null
                                     || Implementation == other.ImplementationType
                                     || (other.ImplementationType is { IsConstructedGenericType: true }
                                         && Implementation == other.ImplementationType.GetGenericTypeDefinition());
        return
            other.ServiceType == ServiceType
            && lifeTimeEquality
            && implementationEquality;
    }

    public override string ToString() => new TypeDescriptorString(this);

    public void Deconstruct(out ServiceLifetime? Lifetime, out Type? Implementation)
    {
        Lifetime = this.Lifetime;
        Implementation = this.Implementation;
    }
}