using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockTempInfo : ITempInfo
    {
        private MockValueProvider currentCoreTempProvider = new MockValueProvider(20, 80);
        public decimal CurrentCoreTemp => currentCoreTempProvider.Value;

        public decimal CurrentTargetTemp { get; set; } = 80;

        public ValueRange TargetTempRange => new ValueRange(45, 92);
    }
}
