using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Lib.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static System.Windows.Forms.AxHost;

namespace GPUControl.ViewModels
{
    public partial class GpuStatusViewModel : ObservableObject
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
            UpdateState(null);
        }

        private class DynamicStateSnapshot
        {
            // (ew, state duplication)
            public decimal CoreClock, MemoryClock, PowerTarget, CurrentPower, TempTarget, CurrentTemp;
            public uint BusUsagePercent, MemoryUsagePercent, GpuUsagePercent;
            public int NumDisplays, NumDisplayConnections;

            public string PerformanceLimit, CurrentPerformanceState;

            public List<StateData.FanInfo> Fans;
        }

        public void UpdateState(Dispatcher? dispatcher)
        {
            // pulling all the GPU info can be time consuming; put that data fetching on a background thread
            // and then update UI via Dispatcher once that heavy work has been done.

            if (gpu != null)
            {
                State ??= new StateData
                {
                    CoreBaseClock = gpu.Clocks.BaseCoreClockMhz,
                    MemoryBaseClock = gpu.Clocks.BaseMemoryClockMhz,
                    Architecture = gpu.Device.ArchitectureName,
                    PciBusInfo = gpu.Device.PciBusInfo,
                    BiosVersion = gpu.Device.BiosVersion,
                    NumCores = gpu.Device.NumCores,
                    NumRops = gpu.Device.NumRops,
                    VramSizeMB = gpu.Device.VramSizeMB,
                    FanSpeeds = gpu.Fans.Entries.Select(info => new StateData.FanInfo(info)).ToList()
                };

                Action<DynamicStateSnapshot> applyState = (snapshot) =>
                {
                    State.CoreClock = snapshot.CoreClock;
                    State.MemoryClock = snapshot.MemoryClock;
                    State.PowerTarget = snapshot.PowerTarget;
                    State.CurrentPower = snapshot.CurrentPower;
                    State.TempTarget = snapshot.TempTarget;
                    State.CurrentTemp = snapshot.CurrentTemp;
                    State.CurrentPerformanceState = snapshot.CurrentPerformanceState;
                    State.PerformanceLimit = snapshot.PerformanceLimit;
                    State.BusUsagePercent = snapshot.BusUsagePercent;
                    State.MemoryUsagePercent = snapshot.MemoryUsagePercent;
                    State.NumDisplays = snapshot.NumDisplays;
                    State.NumDisplayConnections = snapshot.NumDisplayConnections;

                    foreach (var fan in State.FanSpeeds)
                    {
                        var newData = snapshot.Fans.Single(f => f.Id == fan.Id);
                        fan.FanPercent = newData.FanPercent;
                        fan.FanRpm = newData.FanRpm;
                    }
                };

                Func<DynamicStateSnapshot> buildSnapshot = () =>
                {
                    return new DynamicStateSnapshot()
                    {
                        CoreClock = gpu.Clocks.CurrentCoreClockMhz,
                        MemoryClock = gpu.Clocks.CurrentMemoryClockMhz,
                        PowerTarget = gpu.Power.CurrentTargetPower,
                        CurrentPower = gpu.Power.CurrentPower,
                        TempTarget = gpu.Temps.CurrentTargetTemp,
                        CurrentTemp = gpu.Temps.CurrentCoreTemp,
                        CurrentPerformanceState = gpu.Utilization.CurrentPerformanceState,
                        PerformanceLimit = gpu.Utilization.PerformanceLimit,
                        BusUsagePercent = gpu.Utilization.BusUsagePercent,
                        MemoryUsagePercent = gpu.Utilization.MemoryUsagePercent,
                        GpuUsagePercent = gpu.Utilization.GpuUsagePercent,
                        NumDisplays = gpu.Device.NumConnectedDisplays,
                        NumDisplayConnections = gpu.Device.NumAvailableConnections,
                        Fans = gpu.Fans.Entries.Select(e => new StateData.FanInfo(e)).ToList()
                    };
                };

                if (dispatcher == null)
                {
                    applyState(buildSnapshot());
                }
                else
                {
                    Task.Run(() =>
                    {
                        var snapshot = buildSnapshot();
                        dispatcher.BeginInvoke(() => applyState(snapshot));
                    });
                }
            }
        }

        public string GpuName { get; }

        [ObservableProperty]
        private StateData state;

        public partial class StateData : ObservableObject
        {
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(CoreClockString))]
            private decimal coreClock = 1200;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(CoreBaseClockString))]
            private decimal coreBaseClock = 1000;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(MemoryClockString))]
            private decimal memoryClock = 8000;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(MemoryBaseClockString))]
            private decimal memoryBaseClock = 7500;

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(PowerTargetString))]
            private decimal powerTarget = 100;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(PowerTargetString))]
            private decimal currentPower = 70;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(TempTargetString))]
            private decimal tempTarget = 70;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(TempTargetString))]
            private decimal currentTemp = 40;

            [ObservableProperty]
            private string currentPerformanceState = "P0";
            [ObservableProperty]
            private string performanceLimit = "None";
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(BusUsageString))]
            private uint busUsagePercent = 50;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(MemoryUsageString))]
            private uint memoryUsagePercent = 20;
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(GpuUsageString))]
            private uint gpuUsagePercent = 65;

            [ObservableProperty]
            private string architecture = "GA104";
            [ObservableProperty]
            private string pciBusInfo = "PCI Slot 0";
            [ObservableProperty]
            private string biosVersion = "abcdefg";
            [ObservableProperty]
            private int numCores = 128;
            [ObservableProperty]
            private int numRops = 8;
            [ObservableProperty]
            private int numDisplays = 1;
            [ObservableProperty]
            private int numDisplayConnections = 14;
            [ObservableProperty]
            private int vramSizeMB = 4096;

            public partial class FanInfo : ObservableObject
            {
                public FanInfo() { }
                public FanInfo(IFanInfoEntry entry)
                {
                    Id = entry.Id;
                    FanPercent = (int)entry.FanSpeedPercent;
                    FanRpm = (int)entry.FanSpeedRpm;
                }

                [ObservableProperty]
                private int id = 0;
                [ObservableProperty]
                [NotifyPropertyChangedFor(nameof(Value))]
                private int fanPercent = 50;
                [ObservableProperty]
                [NotifyPropertyChangedFor(nameof(Value))]
                private int fanRpm = 1200;

                public string Label => $"Fan {Id}";
                public string Value => $"{FanPercent}%, {FanRpm} RPM";
            }

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(FansVisibility))]
            private List<FanInfo> fanSpeeds = new List<FanInfo> { new FanInfo() };

            private Visibility FansVisibility => FanSpeeds.Any() ? Visibility.Visible : Visibility.Hidden;

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
