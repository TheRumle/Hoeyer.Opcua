using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodeMethodAssigner<T>(IEntityTypeModel<T> entityModel) : IEntityNodeMethodAssigner<T>
{
    private static readonly Type Type = typeof(T);

    public IEnumerable<MethodState> AssignMethods(BaseObjectState entity)
    {
        var nodeMethods = CreateMethods(entity).ToList();
        var errors = VerifyNoDuplicateMethodNames(nodeMethods);

        if (errors.Length != 0)
        {
            throw new AggregateException(errors);
        }

        AssignMethods(nodeMethods, entity);
        return nodeMethods.Select(e => e.Method);
    }

    private IEnumerable<OpcMethodTypeInfo> CreateMethods(BaseObjectState entity)
    {
        return entityModel.Methods
            .Select(method =>
            {
                var browseName = entityModel.MethodNames[method.Name];
                return new OpcMethodTypeInfo(
                    methodName: browseName,
                    parent: entity,
                    returnType: method.ReturnType == typeof(Task) && !method.ReturnType.IsGenericType
                        ? null
                        : method.ReturnType,
                    arguments: method.GetParameters()
                );
            });
    }

    private static void AssignMethods(IEnumerable<IOpcTypeInfo> methods, BaseObjectState entity)
    {
        foreach (var pr in methods.Select(type => type.InstanceState))
        {
            entity.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            entity.AddChild(pr);
        }
    }

    private static Exception[] VerifyNoDuplicateMethodNames(IList<OpcMethodTypeInfo> methods)
    {
        var methodNames = methods.Select(e => e.Method.BrowseName.Name);
        return methodNames
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(name => new InvalidEntityConfigurationException(
                Type.FullName!,
                $"{Type.FullName} has multiple definitions of the following method: {name}"))
            .ToArray<Exception>();
    }
}