using System.Diagnostics.CodeAnalysis;

namespace Hoeyer.OpcUa.Core.Abstractions;

public interface IEntityNodeStructureFactory
{
    IEntityNode Create(ushort applicationNamespaceIndex);
}

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IEntityNodeStructureFactory<in T> : IEntityNodeStructureFactory;