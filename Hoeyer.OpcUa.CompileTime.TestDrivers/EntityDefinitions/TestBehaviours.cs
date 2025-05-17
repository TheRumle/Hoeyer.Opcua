using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hoeyer.OpcUa.Core;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class TestBehaviours
{
    private const string ENTITY_NAME = "$$ENTITY_NAME$$";
    private static readonly Regex InterfaceNameRegex = new(@"interface\s+([A-Za-z_][A-Za-z0-9_]*)\s*");

    public static readonly string[] Interfaces =
    [
        $@"
        [{nameof(OpcUaEntityMethodsAttribute<object>)}<{ENTITY_NAME}>]
         public interface TestInterface
        {{
            Task None(int q, int b, List<int> dict);
            Task<int> Int(int q, int b, List<int> dict);
            Task<string> String(int q, int b, List<int> dict);
        }}
         "
    ];


    public static IEnumerable<ServiceInterfaceSourceCode> GetServiceInterfaceSourceCodeFor(
        EntitySourceCode entitySourceCode)
    {
        return Interfaces.Select(e => e.Replace(ENTITY_NAME, entitySourceCode.Type)).Select(interfaceSourceCode =>
        {
            var interfaceName = InterfaceNameRegex.Match(interfaceSourceCode).Groups[1].Value;
            return new ServiceInterfaceSourceCode(interfaceName, interfaceSourceCode, entitySourceCode);
        });
    }
}