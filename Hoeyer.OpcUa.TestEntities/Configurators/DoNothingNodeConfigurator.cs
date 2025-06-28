using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.TestEntities.Configurators;

/// <summary>
/// A configurator that ensures that all service methods will have at least one method listener configured.
/// </summary>
[OpcUaEntityService(typeof(IPreinitializedNodeConfigurator<>))]
public sealed class DoNothingMethodCallConfigurator<TEntity> : IPreinitializedNodeConfigurator<TEntity>
{
    /// <inheritdoc />
    public void Configure(IManagedEntityNode managed)
    {
        managed.Examine(node =>
        {
            foreach (var method in node.Methods)
            {
                method.OnCallMethod += (context, methodState, inputArguments, outputArguments) =>
                {
                    return ServiceResult.Good;
                };
            }
        });
    }
}