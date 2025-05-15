using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

public static class TestEntities
{
    private static readonly Regex Regex = new(@"class\s+([A-Za-z_][A-Za-z0-9_]*)\s*");

    private static readonly IEnumerable<string> ValidEntityClassDefinitions =
    [
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class SupportedDataTypes
        {
            public List<int> ListField { get; set; } = null!;
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
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class PropertyAccessesTestEntity
        {
            public string S { get; set; }
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class FullyQualifiedLongAttributeName
        {
            public string MyString { get; set; } = "";
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        public enum Position
        {
            OverThere,
            OverHere,
            OnTheMoon
        }
        
        [OpcUaEntity]
        public sealed class EnumSupport
        {
            public Position Position { get; set; }
        }
        """
    ];
    
    private static readonly IEnumerable<string> UnsupportedEntityClassDefinitions =
    [
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class HashSetType
        {
            public HashSet<int> ListField { get; set; } = null!;
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        public enum Position
        {
            OverThere,
            OverHere,
            OnTheMoon
        }
        
        [OpcUaEntity]
        public class DelegateType
        {
            public delegate void ChangePosition(Position oldPosition, Position newPosition);
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class SelfReference
        {
            public SelfReference self {get; set;}
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class FuncReferencingSelf
        {
            public event Func<int, int, FuncReferencingSelf> function;
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class ActionEvent
        {
            public string MyString { get; set; } = "";
            public event Action<int, int> action;
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaEntity]
        public class FuncEvent
        {
            public event Func<int, int, string> function;
        }
        """
    ];


    public static readonly ImmutableHashSet<EntitySourceCode> Valid = ValidEntityClassDefinitions
        .Select(sourceCode => new EntitySourceCode(Regex.Match(sourceCode).Groups[1].Value, sourceCode))
        .ToImmutableHashSet();
    
    public static readonly ImmutableHashSet<EntitySourceCode> UnsupportedTypes = UnsupportedEntityClassDefinitions
        .Select(sourceCode => new EntitySourceCode(Regex.Match(sourceCode).Groups[1].Value, sourceCode))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<EntitySourceCode> All = Valid.Union(UnsupportedTypes);
}