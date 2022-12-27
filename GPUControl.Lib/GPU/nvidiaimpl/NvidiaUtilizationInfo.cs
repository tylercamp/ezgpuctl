using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NvAPIWrapper.GPU;

namespace GPUControl.Lib.GPU.nvidiaimpl
{
    public class NvidiaUtilizationInfo : IUtilizationInfo
    {
        PhysicalGPU gpu;

        public NvidiaUtilizationInfo(PhysicalGPU gpu)
        {
            this.gpu = gpu;
        }

        public string CurrentPerformanceState => gpu.PerformanceStatesInfo.CurrentPerformanceState.StateId.ToString();

        public string PerformanceLimit => gpu.PerformanceControl.CurrentActiveLimit.ToString();

        public uint BusUsagePercent => (uint)gpu.UsageInformation.BusInterface.Percentage;

        public uint MemoryUsagePercent => (uint)gpu.UsageInformation.FrameBuffer.Percentage;

        public uint GpuUsagePercent => (uint)gpu.UsageInformation.GPU.Percentage;
    }
}
