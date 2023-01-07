using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public partial class GpuOverclockFanViewModel : ObservableObject
    {
        public GpuOverclockFanViewModel()
        {
            Id = 0;
            range = new RangeViewModel();
        }

        public GpuOverclockFanViewModel(IFanInfoEntry info, decimal? currentValue)
        {
            Id = info.Id;
            range = new RangeViewModel($"Fan {Id}", new ValueRange(0, 100), currentValue, "%", isRelative: false);
        }

        public int Id { get; }

        [ObservableProperty]
        RangeViewModel range;
    }

    public partial class GpuOverclockViewModel : ObservableObject
    {
        // only for XAML previews!
        public GpuOverclockViewModel()
        {
            GpuLabel = "GPU Label";
            IsReadOnly = false;

            PowerTarget = new RangeViewModel("Power", new ValueRange(0, 100), 0, "%", isRelative: false);
            CoreOffset = new RangeViewModel("Core Offset", new ValueRange(-1000, 1000), 0, " MHz", isRelative: true);
            MemoryOffset = new RangeViewModel("Memory Offset", new ValueRange(-1000, 1000), null, " MHz", isRelative: true);
            Fans = new List<GpuOverclockFanViewModel>() { new GpuOverclockFanViewModel() };

            GpuId = 0;
        }

        public GpuOverclockViewModel(IGpuWrapper gpu, GpuOverclock overclock)
        {
            this.IsReadOnly = false;

            GpuId = gpu.GpuId;

            GpuLabel = gpu.Label;

            PowerTarget = new RangeViewModel("Power", gpu.Power.TargetPowerRange, overclock.PowerTarget, "%", isRelative: false);
            CoreOffset = new RangeViewModel("Core Offset", gpu.Clocks.CoreClockOffsetRangeMhz, overclock.CoreClockOffset, " MHz", isRelative: true);
            MemoryOffset = new RangeViewModel("Memory Offset", gpu.Clocks.MemoryClockOffsetRangeMhz, overclock.MemoryClockOffset, " MHz", isRelative: true);

            var effectiveFanSpeeds = gpu.Fans.Entries.Select((e, i) => i < overclock.FanSpeeds.Count ? overclock.FanSpeeds[i] : null).ToList();
            Fans = effectiveFanSpeeds.Zip(gpu.Fans.Entries).Select(pair =>
            {
                var (speed, entry) = pair;
                return new GpuOverclockFanViewModel(entry, speed);
            }).ToList();
        }

        private GpuOverclockViewModel(IGpuWrapper gpu, GpuOverclock overclock, bool isReadOnly) : this(gpu, overclock)
        {
            this.IsReadOnly = isReadOnly;
        }

        public static GpuOverclockViewModel GetDefault(IGpuWrapper gpu)
        {
            var defaultOverclock = new GpuOverclock()
            {
                GpuId = gpu.GpuId,
                CoreClockOffset = 0,
                MemoryClockOffset = 0,
                PowerTarget = 100,
                FanSpeeds = gpu.Fans.Entries.Select(e => (decimal?)null).ToList()
            };

            return new GpuOverclockViewModel(gpu, defaultOverclock, true);
        }

        public string GpuLabel { get; }

        public bool IsReadOnly { get; }
        public bool IsWriteAllowed => !IsReadOnly;
        
        public uint GpuId { get; }

        public RangeViewModel CoreOffset { get; }
        public RangeViewModel MemoryOffset { get; }
        public RangeViewModel PowerTarget { get; }

        public List<GpuOverclockFanViewModel> Fans { get; }

        public bool IsStock => CoreOffset.Value == 0 && MemoryOffset.Value == 0 && PowerTarget.Value == 100 && Fans.All(f => !f.Range.HasValue);

        public GpuOverclock AsModelObject => new GpuOverclock()
        {
            GpuId = GpuId,
            CoreClockOffset = CoreOffset.Value,
            MemoryClockOffset = MemoryOffset.Value,
            PowerTarget = PowerTarget.Value,
            FanSpeeds = Fans.Select(f => f.Range.Value).ToList()
        };
    }
}
