using System.ComponentModel.DataAnnotations;

namespace Hoeyer.OpcUa.Core.Configuration.Options;

public sealed record CertificateConfigurationOptions
{
    public string PkiRoot { get; init; } = null!;
    public string CertificateSubjectName { get; init; } = null!;
    [Required] public bool? AutoAcceptUntrustedCertificates { get; init; } = null!;
    [Required] public bool? AddAppCertToTrustedStore { get; init; } = null!;
    [Required] public bool? AcceptRejectedCertificates { get; init; } = null!;
}