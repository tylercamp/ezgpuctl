using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpu.mockimpl
{
    public class MockPowerInfo : IPowerInfo
    {
        private MockValueProvider currentPowerProvider = new MockValueProvider(0, 100);
        public decimal CurrentPower => currentPowerProvider.Value;

        private decimal currentTargetPower = 100;
        public decimal CurrentTargetPower
        {
            get => currentTargetPower;
            set
            {
                currentTargetPower = value;
                currentPowerProvider = new MockValueProvider(0, value);
            }
        }

        public ValueRange TargetPowerRange { get; set; } = new ValueRange(40, 105);
    }
}
