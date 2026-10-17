using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.Application;

public sealed class CertificateBasedSecurityConfigurationFactory(
    IApplicationConfigurationRequirements targetServerInfo,
    ITelemetryContext telemetryContext
    )
    : IApplicationSecurityConfigurationFactory
{
    public SecurityConfiguration Configure(ApplicationConfiguration configuration)
    {
        var options = targetServerInfo.CertificateConfiguration;

        var securityConfiguration = configuration.SecurityConfiguration;

        securityConfiguration.ApplicationCertificate ??=
            new CertificateIdentifier();
        

        //TODO
        //securityConfiguration.ApplicationCertificate.Certificate ??=
        //    CertificateFactory
        //        .CreateCertificate(
        //            config.ApplicationUri,
        //            config.ApplicationName,
        //            options.CertificateSubjectName,
        //            null)
        //        .CreateForRSA();

        securityConfiguration.ApplicationCertificate.StoreType =
            options.StoreType;

        securityConfiguration.ApplicationCertificate.StorePath =
            options.OwnStorePath;

        securityConfiguration.TrustedPeerCertificates =
            new CertificateTrustList
            {
                StoreType = options.StoreType,
                StorePath = options.TrustedStorePath
            };

        securityConfiguration.TrustedIssuerCertificates =
            new CertificateTrustList
            {
                StoreType = options.StoreType,
                StorePath = options.IssuerStorePath
            };

        securityConfiguration.RejectedCertificateStore =
            new CertificateTrustList
            {
                StoreType = options.StoreType,
                StorePath = options.RejectedStorePath
            };

        securityConfiguration.AutoAcceptUntrustedCertificates =
            options.AutoAcceptUntrustedCertificates;

        securityConfiguration.AddAppCertToTrustedStore = options.AddAppCertToTrustedStore;

        configuration.CertificateManager ??= new CertificateManager(telemetryContext);

        configuration.SecurityConfiguration = securityConfiguration;

        return securityConfiguration;
    }
}