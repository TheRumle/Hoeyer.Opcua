using Hoeyer.Machines.OpcUa.Configuration.Entities.Configuration;

namespace Hoeyer.Machines.OpcUa.Configuration;


/// <summary>
/// A mapping representing an entity of type <typeparamref name="T"/> and how it should be configured.
/// </summary>
/// <param name="EntityConfiguration">The configuration of the entity, representing a mapping of fields and methods.</param>
/// <typeparam name="T">The Entities which the mapping describes</typeparam>
public sealed record EntityOpcUaMapping<T>(EntityConfiguration<T>  EntityConfiguration, T Entity)
{

    /// <summary>The configuration of the entity, representing a mapping of fields and methods.</summary>
    public EntityConfiguration<T> EntityConfiguration { get; } = EntityConfiguration;

    /// <inheritdoc />
    public T Entity { get; }
}

