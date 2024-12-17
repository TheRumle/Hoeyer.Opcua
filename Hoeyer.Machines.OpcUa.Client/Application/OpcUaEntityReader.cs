using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentResults.Extensions;
using Hoeyer.Common.Extensions.Functional;
using Hoeyer.Machines.OpcUa.Client.Domain;
using Hoeyer.Machines.OpcUa.Client.Infrastructure.Configuration.Entities;
using Hoeyer.Machines.OpcUa.Client.Services.BuildingServices;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.Machines.OpcUa.Client.Application;

public sealed class OpcUaEntityReader<TEntity>(
    DataValuePropertyAssigner<TEntity> assigner,
    EntityConfiguration<TEntity> settings):
    IOpcUaNodeConnectionHolder<TEntity> where TEntity : new()
{
    private readonly List<PropertyConfiguration> _nodes = settings
        .PropertyConfigurations
        .ToList();

    /// <inheritdoc />
    public async Task<Result<TEntity>> ReadOpcUaEntityAsync(Session session)
    {
        return await _nodes.Select(propertyConfiguration => CreateDataMatch(session, propertyConfiguration))
            .Traverse()
            .Bind(possibleMatches => assigner.AssignValuesToInstance(() => new TEntity(), possibleMatches));
    }

    private static Task<Result<PossiblePropertyDataMatch>> CreateDataMatch(Session session, PropertyConfiguration propertyConfiguration)
    {
        return session.ReadValueAsync(propertyConfiguration.GetNodeId())
            .Traverse()
            .Map(dataValue => new PossiblePropertyDataMatch(
                propertyConfiguration,
                dataValue
            ));
    }
}