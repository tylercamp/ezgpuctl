using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockFanInfo : IFanInfo
    {
        public List<IFanInfoEntry> Entries => new List<IFanInfoEntry>() { new MockFanInfoEntry(1) };
    }

    public class MockFanInfoEntry : IFanInfoEntry
    {
        public MockFanInfoEntry(int id)
        {
            Id = id;
        }

        public decimal FanSpeedPercent => MockValueProvider.Percent.Value;

        private MockValueProvider rpmProvider = new MockValueProvider(500, 3000);
        public decimal FanSpeedRpm => rpmProvider.Value;

        public int Id { get; }
    }
}
