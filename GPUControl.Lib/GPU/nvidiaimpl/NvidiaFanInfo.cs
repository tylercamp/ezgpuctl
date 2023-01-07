using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.nvidiaimpl
{
    public class NvidiaFanInfo : IFanInfo
    {
        PhysicalGPU _gpu;

        public NvidiaFanInfo(PhysicalGPU gpu)
        {
            this._gpu = gpu;
        }

        internal IEnumerable<GPUCooler> CoolerInfo => _gpu.CoolerInformation.Coolers.Where(c => c.CoolerType == NvAPIWrapper.Native.GPU.CoolerType.Fan);

        public List<IFanInfoEntry> Entries => CoolerInfo.Select(i => (IFanInfoEntry)new NvidiaFanInfoEntry(i)).ToList();
    }

    public class NvidiaFanInfoEntry : IFanInfoEntry
    {
        GPUCooler cooler;

        public NvidiaFanInfoEntry(GPUCooler cooler)
        {
            this.cooler = cooler;
        }

        public decimal FanSpeedPercent => cooler.CurrentLevel;

        public decimal FanSpeedRpm => cooler.CurrentFanSpeedInRPM;

        public int Id => cooler.CoolerId;
    }
}
