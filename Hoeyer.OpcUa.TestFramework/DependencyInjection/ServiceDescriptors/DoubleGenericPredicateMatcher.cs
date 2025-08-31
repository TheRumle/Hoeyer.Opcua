using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.Opc.Ua.Test.TUnit.DependencyInjection.ServiceDescriptors;

public sealed record DoubleGenericPredicateMatcher : IPartialServiceMatcher
{
    /// <summary>
    ///     Used to compare two types generic types based on generic type definitions.
    /// </summary>
    /// <param name="genericService">
    ///     The generic service <see cref="Type" /> to match against. Must be a generic type or a constructed generic type.
    /// </param>
    /// <param name="genericImplementation">
    ///     The generic implementation <see cref="Type" /> to match against. Must be a generic type or a constructed generic
    ///     type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="genericService" /> or <paramref name="genericImplementation" /> is not a generic type.
    /// </exception>
    public DoubleGenericPredicateMatcher(Type genericService, Type genericImplementation)
    {
        if (!genericService.IsGenericType)
        {
            throw new ArgumentNullException(nameof(genericService) + " must be a generic type");
        }

        ServiceType = genericService.IsConstructedGenericType
            ? genericService.GetGenericTypeDefinition()
            : genericService;

        if (!genericImplementation.IsGenericType)
        {
            throw new ArgumentNullException(nameof(genericService) + " must be a generic type");
        }

        Implementation = genericImplementation.IsConstructedGenericType
            ? genericImplementation.GetGenericTypeDefinition()
            : genericImplementation;
    }

    public Type ServiceType { get; init; }

    public ServiceLifetime Lifetime { get; init; }

    public Type Implementation { get; init; }


    /// <inheritdoc />
    public bool Equals(ServiceDescriptor? other)
    {
        if (other == null)
        {
            return false;
        }

        var otherImplType = other.ImplementationType ?? other.ImplementationInstance?.GetType();
        if (otherImplType is { IsGenericType: false })
        {
            return false;
        }

        if (other.ServiceType is { IsGenericType: false })
        {
            return false;
        }

        var exactImplMatch =
            (otherImplType is { IsConstructedGenericType: true } &&
             otherImplType.GetGenericTypeDefinition() == Implementation) ||
            (otherImplType is { IsGenericTypeDefinition: true } &&
             otherImplType == Implementation);

        var exactServiceMatch =
            (other.ServiceType is { IsConstructedGenericType: true } &&
             other.ServiceType.GetGenericTypeDefinition() == ServiceType) ||
            (other.ServiceType is { IsGenericTypeDefinition: true } &&
             other.ServiceType == ServiceType);

        return exactServiceMatch && exactImplMatch;
    }
}