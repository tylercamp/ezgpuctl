using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl
{
    /// <summary>
    /// Provides utilities for user-facing actions
    /// </summary>
    public class GpuWrapper
    {
        PhysicalGPU gpu;

        public GpuWrapper(PhysicalGPU gpu)
        {
            this.gpu = gpu;

            Clocks = new gpuwrapper.ClockInfo(gpu);
            Power = new gpuwrapper.PowerInfo(gpu);
            Temps = new gpuwrapper.TempInfo(gpu);
        }

        public uint GpuId => gpu.GPUId;

        public gpuwrapper.ClockInfo Clocks { get; }
        public gpuwrapper.PowerInfo Power { get; }
        public gpuwrapper.TempInfo Temps { get; }
    }
}
