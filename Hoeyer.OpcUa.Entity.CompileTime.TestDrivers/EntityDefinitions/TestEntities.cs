using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class TestEntities
{
    private static Regex regex = new(@"class\s+([A-Za-z_][A-Za-z0-9_]*)\s*");

    private static IEnumerable<string> ValidEntityClassDefinitions =
    [
        """
        using System;
        using System.Collections.Generic;
        [OpcUaEntity]
        public class SupportedDataTypes
        {
            public List<int> ListField { get; set; } = null!;
            public HashSet<int> HashSetField { get; set; } = null!;
            public bool boolField { get; set; } = default!;
            public byte byteField { get; set; } = default!;
            public short shortField { get; set; } = default!;
            public ushort ushortField { get; set; } = default!;
            public int intField { get; set; } = default!;
            public uint uintField { get; set; } = default!;
            public long longField { get; set; } = default!;
            public ulong ulongField { get; set; } = default!;
            public float floatField { get; set; } = default!;
            public double doubleField { get; set; } = default!;
            public string stringField { get; set; } = default!;
        }
        """,
        """
        using System;
        using System.Collections.Generic;
        [OpcUaEntity]
        public class PropertyAccessesTestEntity
        {
            public string S { get; set; }
        }
        """,
        """
        [OpcUaEntity]
        public class FullyQualifiedLongAttributeName
        {
            public string MyString { get; set; } = "";
        }
        """
    ];


    public static readonly ImmutableHashSet<EntitySourceCode> Valid = ValidEntityClassDefinitions
        .Select(sourceCode => new EntitySourceCode(regex.Match(sourceCode).Groups[1].Value, sourceCode))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<EntitySourceCode> All = Valid;
}