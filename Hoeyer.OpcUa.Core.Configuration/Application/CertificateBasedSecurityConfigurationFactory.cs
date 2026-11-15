using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Opc.Ua;
using Opc.Ua.Security.Certificates;
using CertificateValidationOptions = Opc.Ua.CertificateValidationOptions;

namespace Hoeyer.OpcUa.Core.Configuration.Application;

internal sealed class CertificateBasedSecurityConfigurationFactory(
    ICertificateFactory certificateFactory,
    IApplicationConfigurationRequirements targetServerInfo,
    ITelemetryContext telemetryContext)
    : IApplicationSecurityConfigurationFactory
{
    public SecurityConfiguration Configure(
        ApplicationConfiguration configuration)
    {
        var options = targetServerInfo.CertificateConfiguration;

        var certificate = CreateAndStoreCertificate(
            configuration,
            options);

        var securityConfiguration =
            configuration.SecurityConfiguration
            ?? new SecurityConfiguration();

        // Always use the certificate created for this configuration.
        securityConfiguration.ApplicationCertificate = certificate;

        securityConfiguration.AutoAcceptUntrustedCertificates =
            options.AutoAcceptUntrustedCertificates;

        securityConfiguration.AddAppCertToTrustedStore =
            options.AddAppCertToTrustedStore;

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

        configuration.SecurityConfiguration = securityConfiguration;
        configuration.CertificateManager =
            CreateAndConfigureCertificateManager();

        return securityConfiguration;
    }

    private CertificateIdentifier CreateAndStoreCertificate(
        ApplicationConfiguration configuration,
        CertificateConfiguration options)
    {
        var cert = certificateFactory.CreateApplicationCertificate(
            applicationUri: configuration.ApplicationUri!,
            applicationName: configuration.ApplicationName!,
            subjectName: "CN=Test",
            domainNames: ["localhost"]
        ).CreateForRSA();

        var store = new DirectoryCertificateStore(telemetryContext);

        try
        {
            store.Open(options.OwnStorePath);

            store.AddAsync(cert).GetAwaiter().GetResult();
        }
        finally
        {
            store.Close();
        }

        return new CertificateIdentifier
        {
            CertificateType =
                ObjectTypeIds.RsaSha256ApplicationCertificateType,

            ValidationOptions =
                CertificateValidationOptions.SuppressCertificateExpired
                | CertificateValidationOptions.SuppressRevocationStatusUnknown
                | CertificateValidationOptions.SuppressHostNameInvalid,

            StoreType = options.StoreType,
            StorePath = options.OwnStorePath,
            SubjectName = cert.SubjectName.Name
        };
    }

    private CertificateManager CreateAndConfigureCertificateManager()
    {
        var manager = new CertificateManager(telemetryContext);

        if (targetServerInfo.CertificateConfiguration
            .AcceptRejectedCertificates)
        {
            manager.AcceptError += (_, _) => true;
        }

        return manager;
    }
}