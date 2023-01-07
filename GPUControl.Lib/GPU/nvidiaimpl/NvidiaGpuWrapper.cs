using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NvAPIWrapper;

namespace GPUControl.Lib.GPU.nvidiaimpl
{
    /// <summary>
    /// Provides utilities for user-facing actions
    /// </summary>
    public class NvidiaGpuWrapper : IGpuWrapper
    {
        PhysicalGPU gpu;

        public NvidiaGpuWrapper(PhysicalGPU gpu)
        {
            this.gpu = gpu;

            Clocks = new NvidiaClockInfo(gpu);
            Power = new NvidiaPowerInfo(gpu);
            Temps = new NvidiaTempInfo(gpu);
            Utilization = new NvidiaUtilizationInfo(gpu);
            Device = new NvidiaDeviceInfo(gpu);
            Fans = new NvidiaFanInfo(gpu);
        }

        public override uint GpuId => gpu.GPUId;

        public override IClockInfo Clocks { get; }
        public override IPowerInfo Power { get; }
        public override ITempInfo Temps { get; }
        public override IUtilizationInfo Utilization { get; }
        public override IDeviceInfo Device { get; }
        public override IFanInfo Fans { get; }

        public override string Label => $"{gpu.FullName} #{GpuId}";

        public int NumDisplays => gpu.ActiveOutputs.Length;
        public int NumDisplayDevices => gpu.GetDisplayDevices().Length;

        public override void ApplyOC(Model.GpuOverclock oc)
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

            #region Fan Control
            var fans = Fans.Entries;
            List<decimal?> effectiveFanSpeeds = new List<decimal?>(oc.FanSpeeds);
            while (effectiveFanSpeeds.Count < fans.Count)
                effectiveFanSpeeds.Add(null);

            var newCoolersControl = new PrivateFanCoolersControlV1(
                fans.Zip(effectiveFanSpeeds).Select(pair =>
                {
                    var (cooler, speed) = pair;

                    var mode = speed.HasValue ? NvAPIWrapper.Native.GPU.FanCoolersControlMode.Manual : NvAPIWrapper.Native.GPU.FanCoolersControlMode.Auto;
                    return new PrivateFanCoolersControlV1.FanCoolersControlEntry((uint)cooler.Id, mode);
                }).ToArray()
            );
            GPUApi.SetClientFanCoolersControl(gpu.Handle, newCoolersControl);

            foreach (var (cooler, speed) in fans.Zip(effectiveFanSpeeds))
            {
                var level = speed.HasValue
                    ? new PrivateCoolerLevelsV1.CoolerLevel(NvAPIWrapper.Native.GPU.CoolerPolicy.Manual, (uint)speed.Value)
                    : new PrivateCoolerLevelsV1.CoolerLevel(NvAPIWrapper.Native.GPU.CoolerPolicy.None);

                var levels = new PrivateCoolerLevelsV1(new[] { level });
                try
                {
                    GPUApi.SetCoolerLevels(gpu.Handle, (uint)cooler.Id, levels, 1);
                }
                // we're not always able to set fan speeds, but there's nothing we can check to figure that out. just swallow any exceptions
                catch (Exception e) { }
        }

            #endregion
        }

        public static void Initialize()
        {
            NVIDIA.Initialize();
        }

        public static void Unload()
        {
            NVIDIA.Unload();
        }

        public static List<NvidiaGpuWrapper> GetAll()
        {
            return PhysicalGPU.GetPhysicalGPUs().Select(gpu => new NvidiaGpuWrapper(gpu)).ToList();
        }
    }
}
