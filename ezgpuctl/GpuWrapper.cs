using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native;
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

        public void ApplyOC(Model.GpuOverclock oc)
        {
            #region Power target
            var updatedPolicyEntries = GPUApi.ClientPowerPoliciesGetStatus(gpu.Handle).PowerPolicyStatusEntries.Select(e =>
            {
                if (e.PerformanceStateId != NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance)
                {
                    return e;
                }
                else
                {
                    var result = new PrivatePowerPoliciesStatusV1.PowerPolicyStatusEntry(NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance, (uint)((oc.PowerTarget ?? 100) * 1000));
                    return result;
                }
            }).ToArray();

            var newPolicy = new PrivatePowerPoliciesStatusV1(updatedPolicyEntries);
            GPUApi.ClientPowerPoliciesSetStatus(gpu.Handle, newPolicy);
            #endregion

            #region Clock Offsets
            var baseStates = GPUApi.GetPerformanceStates20(gpu.Handle);
            var newGpuPerfState = new PerformanceStates20InfoV1.PerformanceState20(
                NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance,
                new PerformanceStates20ClockEntryV1[]
                {
                    new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, new PerformanceStates20ParameterDelta((int)(oc.CoreClockOffset ?? 0) * 1000)),
                    new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory, new PerformanceStates20ParameterDelta((int)(oc.MemoryClockOffset ?? 0) * 1000))
                },
                new PerformanceStates20BaseVoltageEntryV1[] { }
            );

            var newInfo = new PerformanceStates20InfoV3(new PerformanceStates20InfoV1.PerformanceState20[] { newGpuPerfState }, 2, 0);

            GPUApi.SetPerformanceStates20(gpu.Handle, newInfo);
            #endregion

            #region Voltage Control

            //GPUApi.SetCoreVoltageBoostPercent(vm._gpu.Handle, new PrivateVoltageBoostPercentV1);
            #endregion
        }
    }
}
