using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public sealed record CertificateConfiguration
{
    public required string PkiRoot { get; init; }

    public required string CertificateSubjectName { get; init; }
    public required string OwnStorePath { get; init; }

    public required string TrustedStorePath { get; init; }

    public required string IssuerStorePath { get; init; }

    public required string RejectedStorePath { get; init; }

    public string StoreType { get; init; } = CertificateStoreType.Directory;

    public bool AutoAcceptUntrustedCertificates { get; init; } = true;

    public bool AddAppCertToTrustedStore { get; init; } = true;

    public bool AcceptRejectedCertificates { get; init; } = true;
}