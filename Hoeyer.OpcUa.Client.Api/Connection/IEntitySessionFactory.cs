using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Connection;

public interface IEntitySessionFactory
{
    /// <summary>
    /// Creates or gets a session for the given client indicated by the <see cref="clientKey"/>
    /// </summary>
    /// <param name="clientKey">an object used to uniquely identify the client utilizing the session. For instance, an <see cref="Hoeyer.OpcUa.Client.Api.Browsing.IEntityBrowser"/></param>
    /// <param name="token">a token to cancel the creation of a session</param>
    /// <returns>A new or reused <seealso cref="IEntitySession"/> wrapper.</returns>
    Task<IEntitySession> GetSessionForAsync(string clientKey, CancellationToken token = default);

    /// <summary>
    ///     Creates a session with a client key represented by the <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="token">A token to cancelling the operation. The session might be used by other classes using the same key.</param>
    /// <typeparam name="TEntity">A type representing the client key.</typeparam>
    /// <returns></returns>
    Task<IEntitySession> GetSessionForAsync<TEntity>(CancellationToken token = default) =>
        GetSessionForAsync(typeof(TEntity).Name, token);

    /// <summary>
    /// Creates or gets a session for the given client indicated by the <see cref="clientKey"/>
    /// </summary>
    /// <param name="clientKey">an object used to uniquely identify the client utilizing the session. For instance, an <see cref="Hoeyer.OpcUa.Client.Api.Browsing.IEntityBrowser"/></param>
    /// <returns>A new or reused <seealso cref="IEntitySession"/> wrapper.</returns>
    IEntitySession GetSessionFor(string clientKey) => GetSessionForAsync(clientKey, CancellationToken.None).Result;

    /// <summary>
    ///     Creates a session with a client key represented by the <typeparamref name="TEntity" />.
    /// </summary>
    /// <typeparam name="TEntity">A type representing the client key.</typeparam>
    /// <returns></returns>
    IEntitySession GetSessionFor<TEntity>() => GetSessionForAsync<TEntity>().Result;
}