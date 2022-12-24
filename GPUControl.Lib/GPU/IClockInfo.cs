using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface IClockInfo
    {
        decimal CurrentCoreClockMhz { get; }
        decimal CurrentMemoryClockMhz { get; }
        decimal BaseCoreClockMhz { get; }
        decimal BaseMemoryClockMhz { get; }

        ValueRange CoreClockOffsetRangeMhz { get; }
        ValueRange MemoryClockOffsetRangeMhz { get; }

        decimal CoreClockOffset { get; }
        decimal MemoryClockOffset { get; }
    }
}
