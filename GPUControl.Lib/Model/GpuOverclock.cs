using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.Model
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

        public static GpuOverclock Stock(uint gpuId) => new GpuOverclock { GpuId = gpuId, CoreClockOffset = 0, MemoryClockOffset = 0, PowerTarget = 100 };

        public static GpuOverclock Merge(uint gpuId, IEnumerable<GpuOverclock> ocsByPriority)
        {
            var result = new GpuOverclock() { GpuId = gpuId };
            foreach (var oc in ocsByPriority.Reverse().Where(oc => oc.GpuId == gpuId))
            {
                result.CoreClockOffset = oc.CoreClockOffset ?? result.CoreClockOffset;
                result.MemoryClockOffset = oc.MemoryClockOffset ?? result.MemoryClockOffset;
                result.PowerTarget = oc.PowerTarget ?? result.PowerTarget;
            }
            return result;
        }

        public override string ToString()
        {
            return $"GPU {GpuId}: CoreOffset: {CoreClockOffset}, MemoryOffset: {MemoryClockOffset}, PowerTarget: {PowerTarget}";
        }
    }
}
