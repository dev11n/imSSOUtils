using System;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Memory.dll
    /// </summary>
    internal struct MemoryRegionResult
    {
        public UIntPtr CurrentBaseAddress { get; init; }
        public long RegionSize { get; init; }
        public UIntPtr RegionBase { get; init; }
    }
}