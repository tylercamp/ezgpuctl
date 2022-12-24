using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockClockInfo : IClockInfo
    {
        MockValueProvider currentCoreClockProvider = new MockValueProvider(200, 1800);
        public decimal CurrentCoreClockMhz => currentCoreClockProvider.Value;

        MockValueProvider currentMemoryClockProvider = new MockValueProvider(4000, 8000);
        public decimal CurrentMemoryClockMhz => currentMemoryClockProvider.Value;

        public decimal BaseCoreClockMhz => 1200;

        public decimal BaseMemoryClockMhz => 8000;

        public ValueRange CoreClockOffsetRangeMhz => new ValueRange(-1000, 1500);

        public ValueRange MemoryClockOffsetRangeMhz => new ValueRange(-1000, 3000);

        public decimal CoreClockOffset { get; set; } = 0;

        public decimal MemoryClockOffset { get; set; } = 0;
    }
}
