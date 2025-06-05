using System.Collections.Generic;
using System.Linq;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Api;

public readonly record struct EntityNodeStructure
{
    public readonly string EntityName;
    public readonly IReadOnlyDictionary<string, PropertyState> Properties;

    public EntityNodeStructure(string entityName, IDictionary<string, PropertyState> properties)
    {
        EntityName = entityName;
        Properties = properties.ToDictionary(k => k.Key, v => v.Value);
    }

    public IEnumerable<PropertyState> PropertyStates => Properties.Values;
}