using Hoeyer.OpcUa.Core.Abstractions.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class EntityNodeMethodAssigner<T>(
    IMethodStateFactory methodStateFactory,
    IEntityTypeModel<T> entityModel) : IEntityNodeMethodAssigner<T>
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
        return nodeMethods;
    }

    private IEnumerable<MethodState> CreateMethods(BaseObjectState entity)
    {
        return entityModel.Methods
            .Select(method =>
            {
                var browseName = entityModel.MethodNames[method.Name];
                return methodStateFactory.ConstructMethodState(
                    parent: entity,
                    methodName: browseName,
                    returnType: method.ReturnType,
                    arguments: method.GetParameters()
                    );
            });
    }

    private static void AssignMethods(IEnumerable<MethodState> methods, BaseObjectState entity)
    {
        foreach (var pr in methods.Select(type => type))
        {
            entity.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            entity.AddChild(pr);
        }
    }

    private static Exception[] VerifyNoDuplicateMethodNames(IList<MethodState> methods)
    {
        var methodNames = methods.Select(e => e.BrowseName.Name);
        return methodNames
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(name => new InvalidEntityConfigurationException(
                Type.FullName!,
                $"{Type.FullName} has multiple definitions of the following method: {name}"))
            .ToArray<Exception>();
    }
}