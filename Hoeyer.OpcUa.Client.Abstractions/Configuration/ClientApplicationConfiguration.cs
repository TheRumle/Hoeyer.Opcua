using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Configuration;

public sealed class ClientApplicationConfiguration(
    Uri applicationNamespaceUri,
    ApplicationConfiguration template) : ApplicationConfiguration(template)
{
    public Uri ApplicationNamespaceUri => applicationNamespaceUri;

}