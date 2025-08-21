using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Connection;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Core.Api;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Writing;

public sealed class EntityWriter<TEntity>(
    ILogger<IEntityWriter<TEntity>> logger,
    IEntityTranslator<TEntity> translator,
    IEntitySessionFactory factory,
    IEntityBrowser<TEntity> browser) : IEntityWriter<TEntity>
{
    private static string SessionId = typeof(TEntity).Name + "Writer";


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
        var structure = await browser.GetNodeStructure(cancellationToken);
        Dictionary<string, WriteValue> toWrite = new();

        foreach (var (key, value) in entityState)
        {
            var s = structure.Properties[key]!;
            s.Value = value;
            toWrite[key] = CreateWriteValue(s);
        }

        await WriteValues(cancellationToken, toWrite.Values);
    }

    private async Task WriteValues(CancellationToken cancellationToken,
        IEnumerable<WriteValue> valuesToWrite)
    {
        IEntitySession session = await factory.GetSessionForAsync(SessionId, cancellationToken);
        WriteResponse? res =
            await session.Session.WriteAsync(null, new WriteValueCollection(valuesToWrite), cancellationToken);
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
                Value = e.Value
            }
        };
}