using GPUControl.gpu.mockimpl;
using GPUControl.gpu.nvidiaimpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpu
{
    public abstract class IGpuWrapper
    {
        public abstract IClockInfo Clocks { get; }
        public abstract IPowerInfo Power { get; }
        public abstract ITempInfo Temps { get; }

        public abstract uint GpuId { get; }

        public abstract string Label { get; }

        public abstract void ApplyOC(Model.GpuOverclock oc);



        public static bool UseMockGpus { get; set; } = false;

        public static List<IGpuWrapper> ListAll()
        {
            if (UseMockGpus) return MockGpuWrapper.GetAll().Select(gpu => gpu as IGpuWrapper).ToList();
            else return NvidiaGpuWrapper.GetAll().Select(gpu => gpu as IGpuWrapper).ToList();
        }
    }
}
