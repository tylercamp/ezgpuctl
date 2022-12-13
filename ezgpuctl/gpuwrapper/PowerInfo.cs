using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.gpuwrapper
{
    public class PowerInfo
    {
        PhysicalGPU gpu;

        public PowerInfo(PhysicalGPU gpu)
        {
            this.gpu = gpu;
        }

        public decimal CurrentPower => (decimal)gpu
            .PowerTopologyInformation.PowerTopologyEntries
            .Where(e => e.Domain == NvAPIWrapper.Native.GPU.PowerTopologyDomain.GPU)
            .First().PowerUsageInPercent;

        public decimal CurrentTargetPower => (decimal)gpu
            .PerformanceControl.PowerLimitPolicies
            .Where(p => p.PerformanceStateId == NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance)
            .First().PowerTargetInPercent;

        public ValueRange TargetPowerRange
        {
            get
            {
                var info = gpu
                    .PerformanceControl.PowerLimitInformation
                    .Where(p => p.PerformanceStateId == NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance)
                    .First();

                return new ValueRange(info.MinimumPowerInPCM / 1000, info.MaximumPowerInPCM / 1000);
            }
        }
    }
}
