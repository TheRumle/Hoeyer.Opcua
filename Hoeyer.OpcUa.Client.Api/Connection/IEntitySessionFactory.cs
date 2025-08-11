using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Connection;

public interface IAgentSessionFactory
{
    /// <summary>
    /// Creates or gets a session for the given client indicated by the <see cref="clientKey"/>
    /// </summary>
    /// <param name="clientKey">an object used to uniquely identify the client utilizing the session. For instance, an <see cref="Hoeyer.OpcUa.Client.Api.Browsing.IAgentBrowser"/></param>
    /// <param name="token">a token to cancel the creation of a session</param>
    /// <returns>A new or reused <seealso cref="ISession"/> wrapper.</returns>
    Task<IAgentSession> GetSessionForAsync(string clientKey, CancellationToken token = default);

    /// <summary>
    /// Creates or gets a session for the given client indicated by the <see cref="clientKey"/>
    /// </summary>
    /// <param name="clientKey">an object used to uniquely identify the client utilizing the session. For instance, an <see cref="Hoeyer.OpcUa.Client.Api.Browsing.IAgentBrowser"/></param>
    /// <returns>A new or reused <seealso cref="ISession"/> wrapper.</returns>
    IAgentSession GetSessionFor(string clientKey);
}