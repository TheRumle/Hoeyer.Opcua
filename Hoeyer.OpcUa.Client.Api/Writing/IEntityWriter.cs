using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Writing;

public interface IEntityWriter;

public interface IEntityWriter<in T> : IEntityWriter
{
    public Task AssignEntityValues(T entity, CancellationToken cancellationToken = default);

    public Task AssignEntityProperties(IEnumerable<(string propertyName, object propertyValue)> entityState,
        CancellationToken cancellationToken = default);
}