using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Model;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public GpuOverclockViewModel(GpuWrapper gpu, GpuOverclock overclock)
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

        private GpuOverclockViewModel(GpuWrapper gpu, GpuOverclock overclock, bool isReadOnly) : this(gpu, overclock)
        {
            this.IsReadOnly = isReadOnly;
        }

        public static GpuOverclockViewModel GetDefault(PhysicalGPU gpu)
        {
            var defaultOverclock = new Model.GpuOverclock()
            {
                GpuId = gpu.GPUId,
                CoreClockOffset = 0,
                MemoryClockOffset = 0,
                PowerTarget = 100
            };

            return new GpuOverclockViewModel(new GpuWrapper(gpu), defaultOverclock, true);
        }

        public string GpuLabel { get; }

        public bool IsReadOnly { get; }
        public bool IsWriteAllowed => !IsReadOnly;
        
        public uint GpuId { get; }

        public ValueRange CoreClockOffsetRange { get; }
        public ValueRange MemoryClockOffsetRange { get; }
        public ValueRange PowerTargetRange { get; }


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesCoreClockOffset))]
        [NotifyPropertyChangedFor(nameof(CoreClockOffsetDisplayValue))]
        private decimal? coreClockOffset;

        public decimal CoreClockOffsetDisplayValue
        {
            get => coreClockOffset ?? 0;
            set
            {
                coreClockOffset = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CoreClockOffset));
            }
        }

        public bool UsesCoreClockOffset
        {
            get => coreClockOffset.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) coreClockOffset = 0;
                else coreClockOffset = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CoreClockOffset));
                OnPropertyChanged(nameof(CoreClockOffsetDisplayValue));
                OnPropertyChanged(nameof(CanChangeCoreClockOffset));
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesMemoryClockOffset))]
        [NotifyPropertyChangedFor(nameof(MemoryClockOffsetDisplayValue))]
        private decimal? memoryClockOffset;

        public decimal MemoryClockOffsetDisplayValue
        {
            get => memoryClockOffset ?? 0;
            set
            {
                memoryClockOffset = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MemoryClockOffset));
            }
        }

        public bool UsesMemoryClockOffset
        {
            get => memoryClockOffset.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) memoryClockOffset = 0;
                else memoryClockOffset = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(MemoryClockOffset));
                OnPropertyChanged(nameof(MemoryClockOffsetDisplayValue));
                OnPropertyChanged(nameof(CanChangeMemoryClockOffset));
            }
        }

        public bool CanChangeCoreClockOffset => IsWriteAllowed && UsesCoreClockOffset;
        public bool CanChangeMemoryClockOffset => IsWriteAllowed && UsesMemoryClockOffset;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(UsesPowerTarget))]
        [NotifyPropertyChangedFor(nameof(PowerTargetDisplayValue))]
        private decimal? powerTarget;

        public decimal PowerTargetDisplayValue
        {
            get => powerTarget ?? 0;
            set
            {
                powerTarget = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PowerTarget));
            }
        }

        public bool UsesPowerTarget
        {
            get => powerTarget.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) powerTarget = 100;
                else powerTarget = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PowerTarget));
                OnPropertyChanged(nameof(PowerTargetDisplayValue));
                OnPropertyChanged(nameof(CanChangePowerTarget));
            }
        }

        public bool CanChangePowerTarget => IsWriteAllowed && UsesPowerTarget;

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
