using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentResults.Extensions;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Machines.OpcUa.Domain;
using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities;
using Hoeyer.Machines.OpcUa.Services.BuildingServices;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Application;

public sealed class EntityReader<TEntity>(
    DataValuePropertyAssigner<TEntity> assigner,
    EntityConfiguration<TEntity> settings):
    IOpcUaNodeStateReader<TEntity> where TEntity : new()
{
    private readonly List<PropertyConfiguration> _nodes = settings
        .PropertyConfigurations
        .ToList();

    /// <inheritdoc />
    public async Task<Result<TEntity>> ReadOpcUaEntityAsync(Session session)
    {
        return await _nodes.Select(async propertyConfiguration =>
                new PossiblePropertyDataMatch(
                    propertyConfiguration,
                    await session.ReadValueAsync(propertyConfiguration.GetNodeId())
                ))
            .Traverse()
            .Bind(matches => assigner.AssignValuesToInstance(()=>new TEntity(), matches));
    }
}