using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Abstractions.NodeStructure;

public interface IPropertyStateFactory
{
    PropertyState ConstructPropertyState(string browseName, PropertyInfo propertyInfo, BaseInstanceState parent);
}

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IPropertyStateFactory<TEntity> : IPropertyStateFactory;