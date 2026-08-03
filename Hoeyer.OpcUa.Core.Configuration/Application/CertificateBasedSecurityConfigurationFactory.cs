using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.Application;

public sealed class CertificateBasedSecurityConfigurationFactory(IApplicationConfigurationRequirements targetServerInfo)
    : IApplicationSecurityConfigurationFactory
{
    public SecurityConfiguration Configure(ApplicationConfiguration config)
    {
        var options = targetServerInfo.CertificateConfiguration;

        var securityConfiguration =
            config.SecurityConfiguration ?? new SecurityConfiguration();

        securityConfiguration.ApplicationCertificate ??=
            new CertificateIdentifier();

        securityConfiguration.ApplicationCertificate.Certificate ??=
            CertificateFactory
                .CreateCertificate(
                    config.ApplicationUri,
                    config.ApplicationName,
                    options.CertificateSubjectName,
                    null)
                .CreateForRSA();

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

        securityConfiguration.AddAppCertToTrustedStore =
            options.AddAppCertToTrustedStore;

        config.CertificateValidator ??= new CertificateValidator();

        if (options.AcceptRejectedCertificates)
        {
            config.CertificateValidator.CertificateValidation +=
                (_, e) => e.Accept = true;
        }

        config.SecurityConfiguration = securityConfiguration;

        return securityConfiguration;
    }
}