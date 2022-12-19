using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class GpuOverclock
    {
        public uint GpuId { get; set; }
        public decimal? CoreClockOffset { get; set; } = null;
        public decimal? MemoryClockOffset { get; set; } = null;
        public decimal? PowerTarget { get; set; } = null;

        [JsonIgnore]
        public bool HasOcSettings => CoreClockOffset.HasValue || MemoryClockOffset.HasValue || PowerTarget.HasValue;

        public GpuOverclock Clone() => new GpuOverclock() { GpuId = GpuId, CoreClockOffset = CoreClockOffset, MemoryClockOffset = MemoryClockOffset, PowerTarget = PowerTarget };

        public override string ToString()
        {
            return $"GPU {GpuId}: CoreOffset: {CoreClockOffset}, MemoryOffset: {MemoryClockOffset}, PowerTarget: {PowerTarget}";
        }
    }
}
