using System.ComponentModel.DataAnnotations;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;

namespace Hoeyer.OpcUa.Core.Configuration.Options;

public sealed class OpcUaOptions
{
    [Required] public string ServerId { get; init; } = null!;
    [Required] public string ServerName { get; init; } = null!;
    [Required] public string ApplicationUri { get; init; } = null!;
    [Required] public WebProtocol Protocol { get; init; }
    [Required] public string Host { get; init; } = null!;
    [Required] public int Port { get; init; }

    public override string ToString() =>
        $"[OpcUaOptions] " +
        $"ServerId={ServerId ?? "null"}, " +
        $"ServerName={ServerName ?? "null"}, " +
        $"ApplicationUri={ApplicationUri ?? "null"}, " +
        $"Protocol={Protocol.ToString() ?? "null"}, " +
        $"Host={Host ?? "null"}, " +
        $"Port={Port}";
}