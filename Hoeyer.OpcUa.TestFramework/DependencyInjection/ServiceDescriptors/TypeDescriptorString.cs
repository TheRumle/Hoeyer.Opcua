using Hoeyer.Common.Extensions.Types;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public readonly struct TypeDescriptorString(IPartialServiceMatcher Matcher)
{
    private readonly IPartialServiceMatcher _matcher = Matcher;

    public static implicit operator string(TypeDescriptorString s)
    {
        var descriptor = s._matcher;
        var lifeTime = " with lifetime " + descriptor.Lifetime;
        var impl = descriptor.Implementation == null
            ? ""
            : " with implementation type " + descriptor.Implementation.Name;

        if (descriptor.ServiceType.IsGenericTypeDefinition)
        {
            return descriptor.ServiceType.GetFriendlyTypeName() + lifeTime + impl;
        }
        else if (descriptor.ServiceType.IsConstructedGenericType)
        {
            var numParams = descriptor.ServiceType.GetGenericArguments().Length;
            var genericArg = string.Join(",", Enumerable.Range(0, numParams).Select(e => "T" + e));
            return descriptor.ServiceType.GetGenericTypeDefinition().GetFriendlyTypeName() + $"[{genericArg}]" +
                   lifeTime + impl;
        }

        return descriptor.ServiceType.Name + lifeTime + impl;
    }

    public override string ToString() => this;
}