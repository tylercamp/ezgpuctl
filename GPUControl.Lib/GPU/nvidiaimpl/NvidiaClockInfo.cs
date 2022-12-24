using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.nvidiaimpl
{
    public class NvidiaClockInfo : IClockInfo
    {
        PhysicalGPU gpu;

        public NvidiaClockInfo(PhysicalGPU gpu)
        {
            this.gpu = gpu;
        }

        public decimal CurrentCoreClockMhz => gpu.CurrentClockFrequencies.GraphicsClock.Frequency / 1000;
        public decimal CurrentMemoryClockMhz => gpu.CurrentClockFrequencies.MemoryClock.Frequency / 1000;

        public decimal BaseCoreClockMhz => gpu.BaseClockFrequencies.GraphicsClock.Frequency / 1000;
        public decimal BaseMemoryClockMhz => gpu.BaseClockFrequencies.MemoryClock.Frequency / 1000;

        public decimal BoostCoreClockMhz => gpu.BoostClockFrequencies.GraphicsClock.Frequency / 1000;
        public decimal BoostMemoryClockMhz => gpu.BoostClockFrequencies.MemoryClock.Frequency / 1000;



        public ValueRange CoreClockOffsetRangeMhz
        {
            get
            {
                var range = GPUApi.GetPerformanceStates20(gpu.Handle)
                    .Clocks[NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance]
                    .Where(c => c.DomainId == NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics)
                    .First()
                    .FrequencyDeltaInkHz.DeltaRange;

                return new ValueRange(range.Minimum / 1000, range.Maximum / 1000);
            }
        }

        public decimal CoreClockOffset
        {
            get
            {
                return GPUApi.GetPerformanceStates20(gpu.Handle)
                    .Clocks[NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance]
                    .Where(c => c.DomainId == NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics)
                    .Where(c => c.IsEditable)
                    .First()
                    .FrequencyDeltaInkHz.DeltaValue / 1000;
            }
        }

        public ValueRange MemoryClockOffsetRangeMhz
        {
            get
            {
                var range = GPUApi.GetPerformanceStates20(gpu.Handle)
                    .Clocks[NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance]
                    .Where(c => c.DomainId == NvAPIWrapper.Native.GPU.PublicClockDomain.Memory)
                    .First()
                    .FrequencyDeltaInkHz.DeltaRange;

                return new ValueRange(range.Minimum / 1000, range.Maximum / 1000);
            }
        }

        public decimal MemoryClockOffset
        {
            get
            {
                return GPUApi.GetPerformanceStates20(gpu.Handle)
                    .Clocks[NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance]
                    .Where(c => c.DomainId == NvAPIWrapper.Native.GPU.PublicClockDomain.Memory)
                    .Where(c => c.IsEditable)
                    .First()
                    .FrequencyDeltaInkHz.DeltaValue / 1000;
            }
        }
    }
}
