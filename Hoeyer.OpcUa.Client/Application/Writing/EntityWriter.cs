using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Abstractions.Writing;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Core.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Writing;

public sealed class EntityWriter<TEntity>(
    ILogger<IEntityWriter<TEntity>> logger,
    IEntityTranslator<TEntity> translator,
    IEntitySessionFactory factory,
    IEntityBrowser<TEntity> browser) : IEntityWriter<TEntity>
{
    public async Task AssignEntityValues(TEntity entity, CancellationToken cancellationToken = default)
    {
        EntityNodeStructure valuesToWrite = await browser.GetNodeStructure(cancellationToken);
        translator.AssignToStructure(entity, valuesToWrite);
        var values = valuesToWrite.Properties.Values.Select(CreateWriteValue);
        await WriteValues(cancellationToken, values);
    }



    private async Task WriteValues(CancellationToken cancellationToken,
        IEnumerable<WriteValue> valuesToWrite)
    {
        var session = await factory.GetSessionForAsync<TEntity>(cancellationToken);
        var res =
            await session.Session.WriteAsync(null, valuesToWrite.ToArrayOf(), cancellationToken);
        foreach (var diagnostic in res.DiagnosticInfos.Where(e => !e.IsNullDiagnosticInfo))
        {
            logger.LogInformation(diagnostic.ToString());
        }
    }

    private static WriteValue CreateWriteValue(PropertyState e) =>
        new()
        {
            AttributeId = Attributes.Value,
            Handle = e.Handle!,
            NodeId = e.NodeId,
            Value = e.Value.GetDataValue()
        };
}

