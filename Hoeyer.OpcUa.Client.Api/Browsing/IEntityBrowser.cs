using System.Threading;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Client.Api.Browsing;

public interface IEntityBrowser<T> : IEntityBrowser;

public interface IEntityBrowser
{
    /// <summary>
    ///     Creates a new IEntity node by browsing the server.
    ///     Traverses the node tree of the OpcUa server to starting from the root. The traversal halts when the node
    ///     representing the entity has been found, browsed and read. A full tree traversal will be skipped if the entity node
    ///     has already been found previously - instead traversal will begin from the root of the node representing the entity.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the browse</param>
    /// <returns>An IEntityNode representing the structure of the entity.</returns>
    Task<IEntityNode> BrowseEntityNode(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a copy of the node structure. Browses the node to discover the structure if this has not been done before.
    /// </summary>
    /// <returns></returns>
    ValueTask<EntityNodeStructure> GetNodeStructure(CancellationToken token = default);
}