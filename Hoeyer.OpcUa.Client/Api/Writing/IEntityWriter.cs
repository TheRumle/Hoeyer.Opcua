using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Api.Writing;

public interface IEntityWriter;
public interface IEntityWriter<in T> : IEntityWriter
{
    public Task AssignEntityValues(T entity, CancellationToken cancellationToken = default);
}
