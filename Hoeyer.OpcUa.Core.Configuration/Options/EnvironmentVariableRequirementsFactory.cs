using Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Configuration.Options;

internal class EnvironmentVariableRequirementsFactory(
    IOptions<OpcUaOptions> uaOptions,
    IOptions<CertificateConfigurationOptions> certificateOptions,
    ILogger<EnvironmentVariableRequirementsFactory> logger) : IApplicationRequirementsFactory
{
    public IApplicationConfigurationRequirements Get()
    {
        if (uaOptions?.Value == null)
        {
            throw new InvalidOperationException($"No {nameof(IOptions<>)} was configured");
        }

        if (uaOptions?.Value == null)
        {
            throw new InvalidOperationException($"No {nameof(IOptions<>)} was configured");
        }

        var uaOptionsValue = uaOptions.Value;
        var certificateOptionsValue = certificateOptions.Value;

        logger.LogInformation(
            "Creating application requirements from OpcUa options {Options} and certificate information with root {certificateOptions}",
            uaOptionsValue, certificateOptionsValue);
        var applicationUri = uaOptionsValue.ApplicationUri.StartsWith('/')
            ? uaOptionsValue.ApplicationUri
            : $"/{uaOptionsValue.ApplicationUri}";


        var certificateValue = CreateCertificateConfig(certificateOptionsValue);

        return ApplicationRequirementBuilder.Create()
            .WithServerId(uaOptionsValue.ServerId)
            .WithServerName(uaOptionsValue.ServerName)
            .WithWebOrigins(uaOptionsValue.Protocol, uaOptionsValue.Host, uaOptionsValue.Port)
            .WithApplicationUri(applicationUri)
            .WithSecurityConfiguration(certificateValue)
            .Build();
    }

    private CertificateConfiguration CreateCertificateConfig(CertificateConfigurationOptions certificateOptionsValue)
    {
        var root = certificateOptionsValue.PkiRoot;
        return new CertificateConfiguration()
        {
            PkiRoot = root,
            CertificateSubjectName = certificateOptionsValue.CertificateSubjectName,
            IssuerStorePath = Path.Combine(root, "Own"),
            OwnStorePath = Path.Combine(root, "Trusted"),
            RejectedStorePath = Path.Combine(root, "Issuer"),
            TrustedStorePath = Path.Combine(root, "Rejected"),
            AcceptRejectedCertificates = certificateOptionsValue.AcceptRejectedCertificates ??
                                         throw new NullReferenceException(
                                             $"{nameof(CertificateConfigurationOptions)} had null value in {nameof(CertificateConfigurationOptions.AcceptRejectedCertificates)}"),
            AddAppCertToTrustedStore = certificateOptionsValue.AddAppCertToTrustedStore ??
                                       throw new NullReferenceException(
                                           $"{nameof(CertificateConfigurationOptions)} had null value in {nameof(CertificateConfigurationOptions.AddAppCertToTrustedStore)}"),
            AutoAcceptUntrustedCertificates = certificateOptionsValue.AutoAcceptUntrustedCertificates ??
                                              throw new NullReferenceException(
                                                  $"{nameof(CertificateConfigurationOptions)} had null value in {nameof(CertificateConfigurationOptions.AutoAcceptUntrustedCertificates)}"),
            StoreType = CertificateStoreType.Directory
        };
    }
}