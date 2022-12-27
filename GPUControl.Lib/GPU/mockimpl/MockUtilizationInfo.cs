using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockUtilizationInfo : IUtilizationInfo
    {
        public string CurrentPerformanceState => "P0";

        public string PerformanceLimit => "No Limit";


        public uint BusUsagePercent => (uint)MockValueProvider.Percent.Value;

        public uint MemoryUsagePercent => (uint)MockValueProvider.Percent.Value;

        public uint GpuUsagePercent => (uint)MockValueProvider.Percent.Value;
    }
}
