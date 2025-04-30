using System;

namespace Hoeyer.Common.Extensions;

public static class RandomExtensions
{
    public static uint GetUInt(this Random random)
    {
        var bytes = new byte[sizeof(uint)];
        random.NextBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
        
    
}