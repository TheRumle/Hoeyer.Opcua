using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Writing;

[OpcUaEntityService(typeof(IEntityWriter<>))]
public sealed class EntityWriter<TEntity>(
    ILogger<IEntityWriter<TEntity>> logger,
    IEntityTranslator<TEntity> translator,
    IEntitySessionFactory factory,
    IEntityBrowser<TEntity> browser) : IEntityWriter<TEntity>
{
    public static readonly string SessionName = typeof(TEntity).Name + "Writer";

    public async Task AssignEntityValues(TEntity entity, CancellationToken cancellationToken = default)
    {
        EntityNodeStructure valuesToWrite = await browser.GetNodeStructure(cancellationToken);
        translator.AssignToStructure(entity, (name, value) => valuesToWrite.Properties[name].Value = value);
        IEnumerable<WriteValue> values = valuesToWrite.Properties.Values.Select(CreateWriteValue);
        await WriteValues(cancellationToken, values);
    }

    /// <inheritdoc />
    public async Task AssignEntityProperties(IEnumerable<(string propertyName, object propertyValue)> entityState,
        CancellationToken cancellationToken = default)
    {
        EntityNodeStructure structure = await browser.GetNodeStructure(cancellationToken);
        foreach (var (key, value) in entityState)
        {
            structure.Properties[key].Value = value;
        }

        IEnumerable<WriteValue> values = structure.PropertyStates.Select(CreateWriteValue);
        await WriteValues(cancellationToken, values);
    }

    private async Task WriteValues(CancellationToken cancellationToken,
        IEnumerable<WriteValue> valuesToWrite)
    {
        ISession session = await factory.CreateSessionAsync(Guid.NewGuid().ToString(), cancellationToken);
        WriteResponse? res = await session.WriteAsync(null, new WriteValueCollection(valuesToWrite), cancellationToken);
        foreach (DiagnosticInfo? s in res.DiagnosticInfos.Where(e => !e.IsNullDiagnosticInfo))
        {
            logger.LogInformation(s.ToString());
        }
    }

    private static WriteValue CreateWriteValue(ValueProperty e) =>
        new()
        {
            AttributeId = Attributes.Value,
            Handle = e.Handle,
            NodeId = e.NodeId,
            Value = new DataValue
            {
                Value = e.Value,
            }
        };
}