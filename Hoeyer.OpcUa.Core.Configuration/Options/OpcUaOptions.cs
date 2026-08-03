using System.ComponentModel.DataAnnotations;
using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

namespace Hoeyer.OpcUa.Core.Configuration.Options;

public sealed record CertificateConfigurationOptions
{
    public string PkiRoot { get; init; } = null!;
    public string CertificateSubjectName { get; init; } = null!;
    [Required] public bool? AutoAcceptUntrustedCertificates { get; init; } = null!;
    [Required] public bool? AddAppCertToTrustedStore { get; init; } = null!;
    [Required] public bool? AcceptRejectedCertificates { get; init; } = null!;
}

public sealed class OpcUaOptions
{
    [Required] public string ServerId { get; init; } = null!;
    [Required] public string ServerName { get; init; } = null!;
    [Required] public string ApplicationUri { get; init; } = null!;
    [Required] public WebProtocol Protocol { get; init; }
    [Required] public string Host { get; init; } = null!;
    [Required] public int Port { get; init; }

    [Required] public CertificateConfigurationOptions CertificateConfiguration { get; init; } = new();

    public override string ToString() =>
        $"[OpcUaOptions] " +
        $"ServerId={ServerId ?? "null"}, " +
        $"ServerName={ServerName ?? "null"}, " +
        $"ApplicationUri={ApplicationUri ?? "null"}, " +
        $"Protocol={Protocol.ToString() ?? "null"}, " +
        $"Host={Host ?? "null"}, " +
        $"Port={Port}, " +
        $"SecurityConfig={CertificateConfiguration?.ToString() ?? "null"}";
}