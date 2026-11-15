using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Abstractions.Configuration;

public record struct ServerConfigurable(ServerConfiguration Configuration);