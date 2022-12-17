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
    public class GpuOverclockViewModel : ViewModel
    {
        // only for XAML previews!
        public GpuOverclockViewModel()
        {
            GpuLabel = "GPU Label";
            IsReadOnly = false;
            _pendingCoreOffset = 0;
            _pendingMemoryOffset = 0;
            _pendingPowerTarget = null;

            PowerTargetRange = new ValueRange(0, 100);
            CoreClockOffsetRange = new ValueRange(-1000, 1000);
            MemoryClockOffsetRange = new ValueRange(-1000, 1000);
        }

        GpuOverclock overclock;
        public GpuOverclockViewModel(GpuWrapper gpu, GpuOverclock overclock)
        {
            this.overclock = overclock;
            this.IsReadOnly = false;

            GpuLabel = gpu.Label;
            this._pendingCoreOffset = overclock.CoreClockOffset;
            this._pendingMemoryOffset = overclock.MemoryClockOffset;
            this._pendingPowerTarget = overclock.PowerTarget;

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
        
        public uint GpuId
        {
            get => overclock.GpuId;
        }

        public ValueRange CoreClockOffsetRange { get; }

        private decimal? _pendingCoreOffset;
        public decimal? CoreClockOffset
        {
            get => _pendingCoreOffset;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                var changedUse = _pendingCoreOffset.HasValue != value.HasValue;

                _pendingCoreOffset = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanges));

                if (changedUse) OnPropertyChanged(nameof(UsesCoreClockOffset));
            }
        }

        public bool UsesCoreClockOffset
        {
            get => _pendingCoreOffset.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) _pendingCoreOffset = overclock.CoreClockOffset ?? 0;
                else _pendingCoreOffset = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CoreClockOffset));
                OnPropertyChanged(nameof(CanChangeCoreClockOffset));
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        public bool CanChangeCoreClockOffset => IsWriteAllowed && UsesCoreClockOffset;

        public ValueRange MemoryClockOffsetRange { get; }

        private decimal? _pendingMemoryOffset;
        public decimal? MemoryClockOffset
        {
            get => _pendingMemoryOffset;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                _pendingMemoryOffset = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        public bool UsesMemoryClockOffset
        {
            get => _pendingMemoryOffset.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) _pendingMemoryOffset = overclock.MemoryClockOffset ?? 0;
                else _pendingMemoryOffset = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(MemoryClockOffset));
                OnPropertyChanged(nameof(HasChanges));
                OnPropertyChanged(nameof(CanChangeMemoryClockOffset));
            }
        }

        public bool CanChangeMemoryClockOffset => IsWriteAllowed && UsesMemoryClockOffset;

        public ValueRange PowerTargetRange { get; }

        private decimal? _pendingPowerTarget;
        public decimal? PowerTarget
        {
            get => _pendingPowerTarget;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                _pendingPowerTarget = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        public bool UsesPowerTarget
        {
            get => _pendingPowerTarget.HasValue;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                if (value) _pendingPowerTarget = overclock.PowerTarget ?? 100;
                else _pendingPowerTarget = null;

                OnPropertyChanged();
                OnPropertyChanged(nameof(PowerTarget));
                OnPropertyChanged(nameof(HasChanges));
                OnPropertyChanged(nameof(CanChangePowerTarget));
            }
        }

        public bool CanChangePowerTarget => IsWriteAllowed && UsesPowerTarget;

        public bool HasChanges
        {
            get =>
                _pendingCoreOffset != overclock.CoreClockOffset ||
                _pendingMemoryOffset != overclock.MemoryClockOffset ||
                _pendingPowerTarget != overclock.PowerTarget;
        }

        public bool IsStock => _pendingCoreOffset == 0 && _pendingMemoryOffset == 0 && _pendingPowerTarget == 100;

        public Model.GpuOverclock OverclockPreview
        {
            get => new()
            {
                GpuId = GpuId,
                CoreClockOffset = CoreClockOffset,
                MemoryClockOffset = MemoryClockOffset,
                PowerTarget = PowerTarget
            };
        }

        public void ApplyChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            overclock.CoreClockOffset = _pendingCoreOffset;
            overclock.MemoryClockOffset = _pendingMemoryOffset;
            overclock.PowerTarget = _pendingPowerTarget;

            OnPropertyChanged(nameof(HasChanges));
        }

        public void RevertChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            _pendingCoreOffset = overclock.CoreClockOffset;
            _pendingMemoryOffset = overclock.MemoryClockOffset;
            _pendingPowerTarget = overclock.PowerTarget;

            OnPropertyChanged(nameof(CoreClockOffset));
            OnPropertyChanged(nameof(MemoryClockOffset));
            OnPropertyChanged(nameof(PowerTarget));

            OnPropertyChanged(nameof(HasChanges));
        }
    }
}
