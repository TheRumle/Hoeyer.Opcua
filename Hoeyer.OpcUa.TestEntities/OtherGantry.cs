﻿using Hoeyer.OpcUa.Core;

namespace Hoeyer.opcUa.TestEntities;

[OpcUaEntity]
public sealed class OtherGantry
{
    public int OtherGantryIntValue { get; set; }
    public string OtherGantryStringValue { get; set; }
    public List<Guid> OtherGantryAListOfSomeSort { get; set; }
}