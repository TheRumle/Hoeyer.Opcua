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

namespace Hoeyer.OpcUa.Client.Application.Writing;

[OpcUaAgentService(typeof(IAgentWriter<>))]
public sealed class AgentWriter<TAgent>(
    ILogger<IAgentWriter<TAgent>> logger,
    IAgentTranslator<TAgent> translator,
    IAgentSessionFactory factory,
    IAgentBrowser<TAgent> browser) : IAgentWriter<TAgent>
{
    private static string SessionId = typeof(TAgent).Name + "Writer";


    public async Task AssignAgentValues(TAgent agent, CancellationToken cancellationToken = default)
    {
        var valuesToWrite = await browser.GetNodeStructure(cancellationToken);
        translator.AssignToStructure(agent, (name, value) => valuesToWrite.Properties[name].Value = value);
        IEnumerable<WriteValue> values = valuesToWrite.Properties.Values.Select(CreateWriteValue);
        await WriteValues(cancellationToken, values);
    }

    /// <inheritdoc />
    public async Task AssignAgentProperties(IEnumerable<(string propertyName, object propertyValue)> agentState,
        CancellationToken cancellationToken = default)
    {
        var structure = await browser.GetNodeStructure(cancellationToken);
        Dictionary<string, WriteValue> toWrite = new();

        foreach (var (key, value) in agentState)
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
        IAgentSession session = await factory.GetSessionForAsync(SessionId, cancellationToken);
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