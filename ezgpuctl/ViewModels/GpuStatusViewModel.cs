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
                    CurrentTemp = gpu.Temps.CurrentCoreTemp
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

            public string CoreClockString => $"{CoreClock} MHz";
            public string CoreBaseClockString => $"{CoreBaseClock} MHz";
            public string MemoryClockString => $"{MemoryClock} MHz";
            public string MemoryBaseClockString => $"{MemoryBaseClock} MHz";
            public string PowerTargetString => $"{CurrentPower}% / {PowerTarget}%";
            public string TempTargetString => $"{CurrentTemp}C / {TempTarget}C";
        }
    }
}
