using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Writing;

public interface IAgentWriter;

public interface IAgentWriter<in T> : IAgentWriter
{
    public Task AssignAgentValues(T agent, CancellationToken cancellationToken = default);

    public Task AssignAgentProperties(IEnumerable<(string propertyName, object propertyValue)> agentState,
        CancellationToken cancellationToken = default);
}