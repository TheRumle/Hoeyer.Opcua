﻿namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions.Correct.Properties;

[OpcUaEntity]
public class NullableProperties
{
    public bool? TestBool { get; set; }
    public byte? TestByte { get; set; }
    public short? TestShort { get; set; }
    public ushort? TestUShort { get; set; }
    public int? TestInt { get; set; }
    public uint? TestUInt { get; set; }
    public long? TestLong { get; set; }
    public ulong? TestULong { get; set; }
    public float? TestFloat { get; set; }
    public double? TestDouble { get; set; }
    public string? TestString { get; set; }
    public decimal? TestDecimal { get; set; }
}