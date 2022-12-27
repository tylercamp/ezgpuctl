using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Lib.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public class GpuStatusViewModel : ObservableObject
    {
        private IGpuWrapper? gpu;
        
        public GpuStatusViewModel() : this(null)
        {
            State = new StateData();
        }

        public GpuStatusViewModel(IGpuWrapper? gpu)
        {
            this.gpu = gpu;

            GpuName = this.gpu?.Label ?? "GPU Name";
            UpdateState();
        }

        public void UpdateState()
        {
            if (gpu != null)
            {
                State = new StateData
                {
                    CoreClock = gpu.Clocks.CurrentCoreClockMhz,
                    CoreBaseClock = gpu.Clocks.BaseCoreClockMhz,
                    MemoryClock = gpu.Clocks.CurrentMemoryClockMhz,
                    MemoryBaseClock = gpu.Clocks.BaseMemoryClockMhz,
                    PowerTarget = gpu.Power.CurrentTargetPower,
                    CurrentPower = gpu.Power.CurrentPower,
                    TempTarget = gpu.Temps.CurrentTargetTemp,
                    CurrentTemp = gpu.Temps.CurrentCoreTemp,
                    CurrentPerformanceState = gpu.Utilization.CurrentPerformanceState,
                    PerformanceLimit = gpu.Utilization.PerformanceLimit,
                    BusUsagePercent = gpu.Utilization.BusUsagePercent,
                    MemoryUsagePercent = gpu.Utilization.MemoryUsagePercent,
                    GpuUsagePercent = gpu.Utilization.GpuUsagePercent,
                    Architecture = gpu.Device.ArchitectureName,
                    PciBusInfo = gpu.Device.PciBusInfo,
                    BiosVersion = gpu.Device.BiosVersion,
                    NumCores = gpu.Device.NumCores,
                    NumRops = gpu.Device.NumRops,
                    NumDisplays = gpu.Device.NumConnectedDisplays,
                    NumDisplayConnections = gpu.Device.NumAvailableConnections,
                    VramSizeMB = gpu.Device.VramSizeMB
                };

                OnPropertyChanged(nameof(State));
            }
        }

        public string GpuName { get; } = "GPU Name";

        public StateData State { get; private set; }

        public class StateData
        {
            public decimal CoreClock { get; set; } = 1200;
            public decimal CoreBaseClock { get; set; } = 1000;
            public decimal MemoryClock { get; set; } = 8000;
            public decimal MemoryBaseClock { get; set; } = 7500;

            public decimal PowerTarget { get; set; } = 100;
            public decimal CurrentPower { get; set; } = 70;
            public decimal TempTarget { get; set; } = 70;
            public decimal CurrentTemp { get; set; } = 40;

            public string CurrentPerformanceState { get; set; } = "P0";
            public string PerformanceLimit { get; set; } = "None";
            public uint BusUsagePercent { get; set; } = 50;
            public uint MemoryUsagePercent { get; set; } = 20;
            public uint GpuUsagePercent { get; set; } = 65;

            public string Architecture { get; set; } = "GA104";
            public string PciBusInfo { get; set; } = "PCI Slot 0";
            public string BiosVersion { get; set; } = "abcdefg";
            public int NumCores { get; set; } = 128;
            public int NumRops { get; set; } = 8;
            public int NumDisplays { get; set; } = 1;
            public int NumDisplayConnections { get; set; } = 14;
            public int VramSizeMB { get; set; } = 4096;

            public string CoreClockString => $"{CoreClock} MHz";
            public string CoreBaseClockString => $"{CoreBaseClock} MHz";
            public string MemoryClockString => $"{MemoryClock} MHz";
            public string MemoryBaseClockString => $"{MemoryBaseClock} MHz";
            public string PowerTargetString => $"{(int)CurrentPower}% / {PowerTarget}%";
            public string TempTargetString => $"{CurrentTemp}C / {TempTarget}C";

            public string BusUsageString => $"{BusUsagePercent}%";
            public string MemoryUsageString => $"{MemoryUsagePercent}%";
            public string GpuUsageString => $"{GpuUsagePercent}%";

            public string VramSizeString => $"{VramSizeMB} MB";
        }
    }
}
