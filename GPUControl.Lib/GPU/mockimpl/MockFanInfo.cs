using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockFanInfo : IFanInfo
    {
        public List<decimal> FanSpeedsPercent => new List<decimal> { MockValueProvider.Percent.Value };

        private MockValueProvider rpmProvider = new MockValueProvider(500, 3000);
        public List<decimal> FanSpeedsRpm => new List<decimal> { rpmProvider.Value };
    }
}
