using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Microsoft.Extensions.DependencyInjection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructureFactory;

[OpcUaAgentService(typeof(IAgentStructureFactory<>), ServiceLifetime.Singleton)]
public class ReflectionBasedAgentStructureFactory<T> : IAgentStructureFactory<T>
{
    private readonly Type _type = typeof(T);

    /// <inheritdoc />
    public IAgent Create(ushort applicationNamespaceIndex)
    {
        var type = typeof(T);
        var agentName = type.Name;
        BaseObjectState agent = new BaseObjectState(null)
        {
            BrowseName = new QualifiedName(agentName, applicationNamespaceIndex),
            NodeId = new NodeId(agentName, applicationNamespaceIndex),
            DisplayName = agentName,
        };
        agent.AccessRestrictions = AccessRestrictionType.None;

        List<OpcPropertyTypeInfo> properties = CreateProperties(type, agent).ToList();
        List<OpcMethodTypeInfo> methods = CreateMethods(type, agent).ToList();
        Exception[] errors = VerifyNoDuplicateMethodNames(methods)
            .Union(VerifyProperties(properties))
            .ToArray();

        if (errors.Length != 0) throw new AggregateException(errors);
        AssignReferences(properties, agent);
        AssignMethods(methods, agent);

        return new Agent(agent,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(methods.Select(e => e.Method)));
    }

    private IEnumerable<Exception> VerifyProperties(IList<OpcPropertyTypeInfo> properties)
    {
        return properties
            .Where(e => e.TypeId is null)
            .Select(e =>
                new InvalidAgentConfigurationException(_type.FullName!,
                    $"The property {e.PropertyInfo.Name} is of type {e.PropertyInfo.PropertyType.FullName} and could not be translated to NodeId representing the type."));
    }

    private IEnumerable<Exception> VerifyNoDuplicateMethodNames(IList<OpcMethodTypeInfo> methods)
    {
        IEnumerable<string> methodNames = methods.Select(e => e.Method.BrowseName.Name);
        List<IGrouping<string, string>> duplicateNames = methodNames.GroupBy(x => x).Where(g => g.Count() > 1).ToList();
        return duplicateNames
            .Select(name => new InvalidAgentConfigurationException(
                _type.FullName!,
                $"{_type.FullName} has multiple definitions of the following method: {name}"));
    }

    private static void AssignMethods(IEnumerable<IOpcTypeInfo> methods, BaseObjectState agent)
    {
        foreach (var pr in methods.Select(type => type.InstanceState))
        {
            agent.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            agent.AddChild(pr);
        }
    }

    private static void AssignReferences(IEnumerable<IOpcTypeInfo> values, BaseObjectState agent)
    {
        var exceptions = new List<Exception>();
        foreach (var pr in values)
        {
            var referenceTypeid = pr switch
            {
                OpcMethodTypeInfo => ReferenceTypeIds.HasComponent,
                OpcPropertyTypeInfo => ReferenceTypeIds.HasProperty,
                var _ => throw new ArgumentOutOfRangeException(pr.GetType().Name + " is not a handled case")
            };

            agent.AddChild(pr.InstanceState);
            agent.AddReference(referenceTypeid, false, pr.InstanceState.NodeId);
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    private static IEnumerable<OpcMethodTypeInfo> CreateMethods(Type agentType, BaseObjectState agent)
    {
        return OpcUaAgentTypes
            .AgentBehaviours
            .Where(behaviourService => behaviourService.agent == agentType)
            .SelectMany(behaviourService => behaviourService.service
                .GetMembers()
                .OfType<MethodInfo>())
            .Select(method => new OpcMethodTypeInfo(
                methodName: method.Name,
                parent: agent,
                returnType: method.ReturnType == typeof(Task) && !method.ReturnType.IsGenericType
                    ? null
                    : method.ReturnType,
                arguments: method.GetParameters()
            ));
    }

    private static IEnumerable<OpcPropertyTypeInfo> CreateProperties(Type agentType, BaseObjectState agent)
    {
        return agentType.GetProperties().Select(e => new OpcPropertyTypeInfo(e, agent));
    }
}