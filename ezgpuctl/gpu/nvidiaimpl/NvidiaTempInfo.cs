using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpu.nvidiaimpl
{
    public class NvidiaTempInfo : ITempInfo
    {
        PhysicalGPU gpu;

        public NvidiaTempInfo(PhysicalGPU gpu)
        {
            this.gpu = gpu;
        }

        public decimal CurrentCoreTemp => gpu
            .ThermalInformation.ThermalSensors
            .Where(s => s.Target == NvAPIWrapper.Native.GPU.ThermalSettingsTarget.GPU)
            .First().CurrentTemperature;

        public decimal CurrentTargetTemp => gpu
            .PerformanceControl.ThermalLimitPolicies
            .Where(p => p.Controller == NvAPIWrapper.Native.GPU.ThermalController.GPU)
            .First().TargetTemperature;

        public ValueRange TargetTempRange
        {
            get
            {
                var limitInfo = gpu
                    .PerformanceControl.ThermalLimitInformation
                    .Where(e => e.Controller == NvAPIWrapper.Native.GPU.ThermalController.GPU)
                    .First();

                return new ValueRange(limitInfo.MinimumTemperature, limitInfo.MaximumTemperature);
            }
        }
    }
}
