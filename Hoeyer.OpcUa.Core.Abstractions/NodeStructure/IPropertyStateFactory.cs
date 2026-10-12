using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IPropertyStateFactory
{
    PropertyState ConstructPropertyState(string browseName, PropertyInfo propertyInfo, BaseInstanceState parent);
}

public interface IPropertyStateFactory<TEntity> : IPropertyStateFactory;