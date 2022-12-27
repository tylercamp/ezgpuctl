using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface IUtilizationInfo
    {
        string CurrentPerformanceState { get; }

        string PerformanceLimit { get; }
        uint BusUsagePercent { get; }
        uint MemoryUsagePercent { get; }
        uint GpuUsagePercent { get; }
    }
}
