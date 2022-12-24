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
    public partial class GpuOverclockViewModel : ObservableObject
    {
        // only for XAML previews!
        public GpuOverclockViewModel()
        {
            GpuLabel = "GPU Label";
            IsReadOnly = false;
            coreClockOffset = 0;
            memoryClockOffset = 0;
            powerTarget = null;

            PowerTargetRange = new ValueRange(0, 100);
            CoreClockOffsetRange = new ValueRange(-1000, 1000);
            MemoryClockOffsetRange = new ValueRange(-1000, 1000);
            GpuId = 0;
        }

        public GpuOverclockViewModel(IGpuWrapper gpu, GpuOverclock overclock)
        {
            this.IsReadOnly = false;

            GpuId = gpu.GpuId;

            GpuLabel = gpu.Label;
            coreClockOffset = overclock.CoreClockOffset;
            memoryClockOffset = overclock.MemoryClockOffset;
            powerTarget = overclock.PowerTarget;

            PowerTargetRange = gpu.Power.TargetPowerRange;
            CoreClockOffsetRange = gpu.Clocks.CoreClockOffsetRangeMhz;
            MemoryClockOffsetRange = gpu.Clocks.MemoryClockOffsetRangeMhz;
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
                PowerTarget = 100
            };

            return new GpuOverclockViewModel(gpu, defaultOverclock, true);
        }

        public string GpuLabel { get; }

        public bool IsReadOnly { get; }
        public bool IsWriteAllowed => !IsReadOnly;
        
        public uint GpuId { get; }

        public ValueRange CoreClockOffsetRange { get; }
        public ValueRange MemoryClockOffsetRange { get; }
        public ValueRange PowerTargetRange { get; }

        private static string? ExtractDigitsPart(string str)
        {
            var pattern = new Regex("([\\+\\-]?\\s*[-0-9]+)");
            return pattern.Match(str).Captures.FirstOrDefault()?.Value;
        }


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesCoreClockOffset))]
        [NotifyPropertyChangedFor(nameof(CoreClockOffsetDisplayValue))]
        [NotifyPropertyChangedFor(nameof(CoreClockOffsetDisplayString))]
        private decimal? coreClockOffset;

        public decimal CoreClockOffsetDisplayValue
        {
            get => coreClockOffset ?? 0;
            set => CoreClockOffset = value;
        }

        private string? GetDisplayString(decimal? value, string suffix, bool relative)
        {
            if (value == null) return null;

            var formatted = relative ? string.Format("{0:+0;-#}", value) : value.ToString();
            return formatted + suffix;
        }
        private string? ParseDisplayString(string? value)
        {
            var str = value?.Trim() ?? "";
            if (str.Length == 0) return null;
            else return ExtractDigitsPart(str);
        }

        public string? CoreClockOffsetDisplayString
        {
            get => GetDisplayString(coreClockOffset, " MHz", true);
            set
            {
                var parsedValue = ParseDisplayString(value);
                if (parsedValue != null) CoreClockOffset = decimal.Parse(parsedValue);
            }
        }

        public bool UsesCoreClockOffset => coreClockOffset.HasValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesMemoryClockOffset))]
        [NotifyPropertyChangedFor(nameof(MemoryClockOffsetDisplayValue))]
        [NotifyPropertyChangedFor(nameof(MemoryClockOffsetDisplayString))]
        private decimal? memoryClockOffset;

        public decimal MemoryClockOffsetDisplayValue
        {
            get => memoryClockOffset ?? 0;
            set => MemoryClockOffset = value;
        }

        public string? MemoryClockOffsetDisplayString
        {
            get => GetDisplayString(memoryClockOffset, " MHz", true);
            set
            {
                var parsedValue = ParseDisplayString(value);
                if (parsedValue != null) MemoryClockOffset = decimal.Parse(parsedValue);
            }
        }

        public bool UsesMemoryClockOffset => memoryClockOffset.HasValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesPowerTarget))]
        [NotifyPropertyChangedFor(nameof(PowerTargetDisplayValue))]
        [NotifyPropertyChangedFor(nameof(PowerTargetDisplayString))]
        private decimal? powerTarget;

        public decimal PowerTargetDisplayValue
        {
            get => powerTarget ?? 0;
            set => PowerTarget = value;
        }

        public string? PowerTargetDisplayString
        {
            get => GetDisplayString(powerTarget, "%", false);
            set
            {
                var parsedValue = ParseDisplayString(value);
                if (parsedValue != null) PowerTarget = decimal.Parse(parsedValue);
            }
        }

        public bool UsesPowerTarget => powerTarget.HasValue;


        public bool IsStock => coreClockOffset == 0 && memoryClockOffset == 0 && powerTarget == 100;

        public GpuOverclock AsModelObject => new GpuOverclock()
        {
            GpuId = GpuId,
            CoreClockOffset = coreClockOffset,
            MemoryClockOffset = memoryClockOffset,
            PowerTarget = powerTarget
        };
    }
}
