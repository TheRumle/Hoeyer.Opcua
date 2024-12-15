using System;
using FluentResults;

namespace Hoeyer.Machines.Proxy;

public interface IEntityPropertyAssigner<TEntity, in TInformationSource>
{
    /// <summary>
    /// Will assign the entity with new information from the source.
    /// </summary>
    /// <param name="entity">The entity to assign values to.</param>
    /// <param name="source">Tuples representing the property to assign value to and the DataValue read from OpcUaServer.</param>
    /// <returns></returns>
    Result<TEntity> TryAssignToEntity(Func<TEntity> instanceFactory, TInformationSource source);
}