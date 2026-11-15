using Hoeyer.OpcUa.Client.Abstractions.Configuration;
using Hoeyer.OpcUa.Client.Abstractions.Discovery;
using Hoeyer.OpcUa.Core.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Application.Discovery;

public sealed class EndpointDiscovery(
    IEnumerable<(string PolicyUri, MessageSecurityMode Mode)> securityModes,
    IEnumerable<UserTokenPolicy> tokenPolicies,
    IClientApplicationConfigurationFactory applicationConfigurationFactory
) : IEndpointDiscovery
{
    private UserTokenPolicy[] SupportedPolicies { get; } = tokenPolicies.ToArray();

    public async Task<IEnumerable<EndpointDescription>> FindSupportedEndpointsAsync(
        Uri uri,
        CancellationToken cancellationToken = default)
    {
        var applicationConfiguration = applicationConfigurationFactory.CreateClientConfiguration();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(uri.ToString());

        using var discoveryClient = await DiscoveryClient.CreateAsync(
            applicationConfiguration,
            uri,
            EndpointConfiguration.Create(applicationConfiguration),
            ct: cancellationToken);

        var endpoints = (await discoveryClient.GetEndpointsAsync(
                default,
                cancellationToken))
            .ToArray() ?? [];

        return endpoints.Where(MatchesEndpoint);
    }

    private bool MatchesEndpoint(EndpointDescription endpoint)
    {
        var supportedSecurityMode = securityModes.Any(SecurityModeIsSupported(endpoint));
        if (!supportedSecurityMode) return false;
        return endpoint.UserIdentityTokens.Any(MatchesTokenPolicy);
    }

    private static Func<(string PolicyUri, MessageSecurityMode Mode), bool> SecurityModeIsSupported(EndpointDescription endpoint)
    {
        return configuration => endpoint.SecurityMode == configuration.Mode
                                && configuration.PolicyUri.Equals(configuration.PolicyUri);
    }

    private bool MatchesTokenPolicy(
        UserTokenPolicy policy)
    {
        return SupportedPolicies.Any(supportedPolicy =>
        {
            var supportedType = supportedPolicy.TokenType;
            var supportedTokenSecurityPolicyUri = supportedPolicy.SecurityPolicyUri;
            return policy.TokenType == supportedType && policy.SecurityPolicyUri == supportedTokenSecurityPolicyUri;
        });
    }
}