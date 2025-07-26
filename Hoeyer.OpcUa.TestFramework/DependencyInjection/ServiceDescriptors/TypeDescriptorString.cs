namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public readonly record struct TypeDescriptorString(IPartialServiceMatcher Matcher)
{
    public static implicit operator string(TypeDescriptorString s)
    {
        var descriptor = s.Matcher;
        var lifeTime = " with lifetime " + descriptor.Lifetime;
        var impl = descriptor.Implementation == null
            ? ""
            : " with implementation type " + descriptor.Implementation.Name;
        if (descriptor.ServiceType.IsConstructedGenericType)
        {
            return descriptor.ServiceType.GetGenericTypeDefinition().Name + lifeTime + impl;
        }

        return descriptor.ServiceType.Name + lifeTime + impl;
    }

    public override string ToString() => this;
}