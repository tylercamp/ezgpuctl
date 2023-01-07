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
        public List<decimal?> FanSpeeds { get; set; } = new List<decimal?>();

        [JsonIgnore]
        public bool HasOcSettings => CoreClockOffset.HasValue || MemoryClockOffset.HasValue || PowerTarget.HasValue || FanSpeeds.Any(s => s.HasValue);

        public static GpuOverclock Stock(uint gpuId) => new GpuOverclock { GpuId = gpuId, CoreClockOffset = 0, MemoryClockOffset = 0, PowerTarget = 100 };

        public static GpuOverclock Merge(uint gpuId, IEnumerable<GpuOverclock> ocsByPriority)
        {
            var result = new GpuOverclock() { GpuId = gpuId };

            foreach (var oc in ocsByPriority.Reverse().Where(oc => oc.GpuId == gpuId))
            {
                result.CoreClockOffset = oc.CoreClockOffset ?? result.CoreClockOffset;
                result.MemoryClockOffset = oc.MemoryClockOffset ?? result.MemoryClockOffset;
                result.PowerTarget = oc.PowerTarget ?? result.PowerTarget;

                while (result.FanSpeeds.Count < oc.FanSpeeds.Count) result.FanSpeeds.Add(null);

                for (int i = 0; i < result.FanSpeeds.Count && i < oc.FanSpeeds.Count; i++)
                    result.FanSpeeds[i] = oc.FanSpeeds[i] ?? result.FanSpeeds[i];
            }
            return result;
        }

        public override string ToString()
        {
            return $"GPU {GpuId}: Core Offset: {CoreClockOffset}, Mem Offset: {MemoryClockOffset}, Power: {PowerTarget}";
        }
    }
}
