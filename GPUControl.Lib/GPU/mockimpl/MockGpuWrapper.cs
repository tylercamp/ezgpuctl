using GPUControl.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockGpuWrapper : IGpuWrapper
    {
        public MockGpuWrapper(uint id, string name)
        {
            GpuId = id;
            Label = name;

            Clocks = new MockClockInfo();
            Power = new MockPowerInfo();
            Temps = new MockTempInfo();
            Utilization = new MockUtilizationInfo();
            Device = new MockDeviceInfo();
            Fans = new MockFanInfo();
        }

        public override IClockInfo Clocks { get; }

        public override IPowerInfo Power { get; }

        public override ITempInfo Temps { get; }

        public override IUtilizationInfo Utilization { get; }
        public override IDeviceInfo Device { get; }
        public override IFanInfo Fans { get; }

        public override uint GpuId { get; }

        public override string Label { get; }

        public override void ApplyOC(GpuOverclock oc)
        {
            var mockClocks = (Clocks as MockClockInfo)!;
            var mockPower = (Power as MockPowerInfo)!;

            if (oc.CoreClockOffset.HasValue)
                mockClocks.CoreClockOffset = oc.CoreClockOffset.Value;

            if (oc.MemoryClockOffset.HasValue)
                mockClocks.MemoryClockOffset = oc.MemoryClockOffset.Value;

            if (oc.PowerTarget.HasValue)
                mockPower.CurrentTargetPower = oc.PowerTarget.Value;
        }

        public static List<MockGpuWrapper> GetAll()
        {
            return new List<MockGpuWrapper>()
            {
                new MockGpuWrapper(1, "RTX 3060 Ti"),
                new MockGpuWrapper(2, "RTX 3080")
            };
        }
    }
}
