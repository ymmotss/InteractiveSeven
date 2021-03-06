﻿using System;

namespace InteractiveSeven.Core.Diagnostics.Memory
{
    public interface IMemoryAccessor
    {
        bool ReadMem(string processName, IntPtr address, byte[] buffer);
        void WriteMem(string processName, IntPtr address, byte[] bytes);
        ScanResult ScanMem(string processName, IntPtr startAddr,
            ushort itemSize, uint capacity, Func<byte[], bool> isMatch);
    }
}