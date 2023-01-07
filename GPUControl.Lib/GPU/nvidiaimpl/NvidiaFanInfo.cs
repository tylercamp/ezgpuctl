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

        public List<decimal> FanSpeedsPercent => CoolerInfo.Select(c => (decimal)c.CurrentLevel).ToList();

        public List<decimal> FanSpeedsRpm => CoolerInfo.Select(c => (decimal)c.CurrentFanSpeedInRPM).ToList();
    }
}
