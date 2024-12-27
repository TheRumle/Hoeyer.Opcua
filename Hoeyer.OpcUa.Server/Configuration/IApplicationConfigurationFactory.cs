using System;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server;

public interface IApplicationConfigurationFactory
{
    public string ApplicationName { get; }
    public Uri BaseAddress{ get; }

    /// <param name="subject">The unique name identifying the subject to create certificates to.</param>
    /// <param name="legalDomains">The domains that are allowed to access the server</param>
    /// See <see href="https://reference.opcfoundation.org/Core/Part6/v105/docs/6.2"/> for information about <paramref name="subject"/> and its further detail 
    public ApplicationConfiguration CreateServerConfiguration(string subject, params string[] legalDomains);
}