using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures.AgentDefinitions;

public static class AgentSourceCodeDefinitions
{
    private static readonly Regex ClassNameRegex = new(@"class\s+([A-Za-z_][A-Za-z0-9_]*)\s*");

    private static readonly IEnumerable<string> ValidAgentClassDefinitions =
    [
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaAgent]
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
        [OpcUaAgent]
        public class PropertyAccessesTestAgent
        {
            public string S { get; set; }
        }
        """,
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaAgent]
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

        [OpcUaAgent]
        public sealed class EnumSupport
        {
            public Position Position { get; set; }
        }
        """
    ];

    private static readonly IEnumerable<string> UnsupportedAgentClassDefinitions =
    [
        """
        namespace Test;
        using System;
        using System.Collections.Generic;
        using Hoeyer.OpcUa.Core;
        [OpcUaAgent]
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

        [OpcUaAgent]
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
        [OpcUaAgent]
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
        [OpcUaAgent]
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
        [OpcUaAgent]
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
        [OpcUaAgent]
        public class FuncEvent
        {
            public event Func<int, int, string> function;
        }
        """
    ];


    public static readonly ImmutableHashSet<AgentSourceCode> ValidEntities = ValidAgentClassDefinitions
        .Select(sourceCode => new AgentSourceCode(ClassNameRegex.Match(sourceCode).Groups[1].Value, sourceCode))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<AgentSourceCode> UnsupportedTypes = UnsupportedAgentClassDefinitions
        .Select(sourceCode => new AgentSourceCode(ClassNameRegex.Match(sourceCode).Groups[1].Value, sourceCode))
        .ToImmutableHashSet();

    public static readonly ImmutableHashSet<AgentSourceCode> All = ValidEntities.Union(UnsupportedTypes);
}