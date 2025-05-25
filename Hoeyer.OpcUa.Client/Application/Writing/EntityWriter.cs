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
    private IEntityNode? ValuesToWrite { get; set; }

    public async Task AssignEntityValues(TEntity entity, CancellationToken cancellationToken = default)
    {
        ISession session = await factory.CreateSessionAsync("WRITER", cancellationToken);

        //Only fetch the first time - then reuse the structure to write to the node
        ValuesToWrite ??= browser.LastState?.node ?? await browser.BrowseEntityNode(cancellationToken);
        translator.AssignToNode(entity, ValuesToWrite);

        var values = ValuesToWrite.PropertyStates.Select(e => new WriteValue
        {
            AttributeId = Attributes.Value,
            Handle = e.Handle,
            NodeId = e.NodeId,
            Value = new DataValue
            {
                Value = e.Value,
            }
        });
        var res = await session.WriteAsync(null, new WriteValueCollection(values), cancellationToken);
        foreach (DiagnosticInfo? s in res.DiagnosticInfos.Where(e => !e.IsNullDiagnosticInfo))
        {
            logger.LogInformation(s.ToString());
        }
    }
}