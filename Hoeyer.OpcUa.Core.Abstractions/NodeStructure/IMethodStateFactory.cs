using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IMethodStateFactory
{
    MethodState ConstructMethodState(
        BaseObjectState parent, string methodName,
        IEnumerable<ParameterInfo> arguments,
        Type? returnType
    );
}