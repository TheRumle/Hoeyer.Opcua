using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.OpcTypeMappers;

public sealed class VariantConverterCollection(Dictionary<Type, object> collection)
{
    
    public IVariantBuilder<T> Get<T>()
    {
        if (collection.TryGetValue(typeof(T), out var converter))
        {
            if (converter is IVariantBuilder<T> builder)
            {
                return builder;
            }

            throw new InvalidOperationException(
                $"The converter is of type {converter.GetType().Name} which is not assignable to {nameof(IVariantBuilder<>)}<{typeof(T).Name}>");

        }
        
        throw new InvalidOperationException($"No {nameof(IVariantBuilder<>)} has been added for the type {typeof(T).Name}");
    }
}
