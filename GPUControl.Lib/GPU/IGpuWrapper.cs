using GPUControl.Lib.GPU.mockimpl;
using GPUControl.Lib.GPU.nvidiaimpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public abstract class IGpuWrapper
    {
        public abstract IClockInfo Clocks { get; }
        public abstract IPowerInfo Power { get; }
        public abstract ITempInfo Temps { get; }
        public abstract IUtilizationInfo Utilization { get; }
        public abstract IDeviceInfo Device { get; }

        public abstract uint GpuId { get; }

        public abstract string Label { get; }

        public abstract void ApplyOC(Model.GpuOverclock oc);

        public static bool UseMockGpus { get; set; } = false;

        public static bool InitializeAll(Action<Type, Exception> onException)
        {
            if (!UseMockGpus)
            {
                bool initializedAny = false;

                try
                {
                    NvidiaGpuWrapper.Initialize();
                    initializedAny = true;
                }
                catch (Exception e)
                {
                    onException(typeof(NvidiaGpuWrapper), e);
                }

                return initializedAny;
            }
            else
            {
                return true;
            }
        }

        public static void UnloadAll()
        {
            if (!UseMockGpus)
            {
                NvidiaGpuWrapper.Unload();
            }
        }

        public static List<IGpuWrapper> ListAll()
        {
            if (UseMockGpus) return MockGpuWrapper.GetAll().Select(gpu => gpu as IGpuWrapper).ToList();
            else return NvidiaGpuWrapper.GetAll().Select(gpu => gpu as IGpuWrapper).ToList();
        }
    }
}
