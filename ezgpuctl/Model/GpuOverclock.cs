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
    }
}
