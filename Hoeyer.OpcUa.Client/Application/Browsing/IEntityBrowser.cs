using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.MachineProxy;
using Opc.Ua.Client;

namespace Hoeyer.OpcUa.Client.Application.Browsing;

public interface IEntityBrowser<T> : IEntityBrowser;

public interface IEntityBrowser
{
    /// <summary>
    /// Traverses the node tree of the OpcUa server the <paramref name="session"/> is connected to starting from the root. The traversal halts when the node representing the entity has been found and browsed. A full tree traversal will be skipped if the entity node has already been found previously.
    /// </summary>
    /// <param name="session">The session connected to the OpcUa server. See <seealso cref="IEntitySessionFactory"/> <seealso cref="Session"/> </param>
    /// <param name="cancellationToken">Token to cancel the browse</param>
    /// <returns></returns>
    Task<EntityReadResult> BrowseEntityNode(
        ISession session,
        CancellationToken cancellationToken = default);
}