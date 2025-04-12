using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application;

public interface IEntityWriter
{
    public Task WriteNode(ISession session, WriteValueCollection toWrite, CancellationToken cancellationToken = default);
}

public interface IEntityWriter<in T> : IEntityWriter
{
    public Task AssignEntityValues(ISession session, T entity, CancellationToken cancellationToken = default);
};
