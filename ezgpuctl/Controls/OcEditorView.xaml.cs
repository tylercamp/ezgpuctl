using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.GPU.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GPUControl.Controls
{
    public class OcEditorViewModel : ViewModel
    {
        public OcEditorViewModel()
        {
            PowerTargetRange = new ValueRange(0, 100);
            VoltageRange = new ValueRange(0, 100);
            GpuClockRange = new ValueRange(-1000, 1000);
            MemoryClockRange = new ValueRange(-1000, 1000);
            TempTargetRange = new ValueRange(50, 80);

            PowerTarget = PowerTargetRange.Middle;
            Voltage = VoltageRange.Middle;
            GpuClock = GpuClockRange.Middle;
            MemoryClock = MemoryClockRange.Middle;
            TempTarget = TempTargetRange.Middle;
        }

        public PhysicalGPU _gpu;

        public OcEditorViewModel(PhysicalGPU gpu)
        {
            _gpu = gpu;
            var wrapper = new GpuWrapper(gpu);
            this.PowerTargetRange = wrapper.Power.TargetPowerRange;
            this.PowerTarget = wrapper.Power.CurrentTargetPower;

            this.VoltageRange = new ValueRange(0, 100);
            this.Voltage = 0;

            this.GpuClockRange = wrapper.Clocks.CoreClockOffsetRangeMhz;
            this.GpuClock = wrapper.Clocks.CoreClockOffset;

            this.MemoryClockRange = wrapper.Clocks.MemoryClockOffsetRangeMhz;
            this.MemoryClock = wrapper.Clocks.MemoryClockOffset;

            this.TempTargetRange = wrapper.Temps.TempTargetRange;
            this.TempTarget = wrapper.Temps.TempTarget;
        }

        public ValueRange PowerTargetRange { get; }
        public ValueRange VoltageRange { get; }
        public ValueRange GpuClockRange { get; }
        public ValueRange MemoryClockRange { get; }
        public ValueRange TempTargetRange { get; }

        private decimal _powerTarget;
        public decimal PowerTarget
        {
            get => _powerTarget;
            set
            {
                _powerTarget = value;
                OnPropertyChanged();
            }
        }

        private decimal _voltage;
        public decimal Voltage
        {
            get => _voltage;
            set
            {
                _voltage = value;
                OnPropertyChanged();
            }
        }

        private decimal _gpuClock;
        public decimal GpuClock
        {
            get => _gpuClock;
            set
            {
                _gpuClock = value;
                OnPropertyChanged();
            }
        }

        private decimal _memoryClock;
        public decimal MemoryClock
        {
            get => _memoryClock;
            set
            {
                _memoryClock = value;
                OnPropertyChanged();
            }
        }

        private decimal _tempTarget;
        public decimal TempTarget
        {
            get => _tempTarget;
            set
            {
                _tempTarget = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Interaction logic for OcEditorView.xaml
    /// </summary>
    public partial class OcEditorView : UserControl
    {
        public OcEditorView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as OcEditorViewModel;

            #region Power target
            var updatedPolicyEntries = GPUApi.ClientPowerPoliciesGetStatus(vm._gpu.Handle).PowerPolicyStatusEntries.Select(e =>
            {
                if (e.PerformanceStateId != NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance)
                {
                    return e;
                }
                else
                {
                    var result = new PrivatePowerPoliciesStatusV1.PowerPolicyStatusEntry(NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance, (uint)(vm.PowerTarget * 1000));
                    return result;
                }
            }).ToArray();

            var newPolicy = new PrivatePowerPoliciesStatusV1(updatedPolicyEntries);
            GPUApi.ClientPowerPoliciesSetStatus(vm._gpu.Handle, newPolicy);
            #endregion

            #region Clock Offsets
            var baseStates = GPUApi.GetPerformanceStates20(vm._gpu.Handle);
            var newGpuPerfState = new PerformanceStates20InfoV1.PerformanceState20(
                NvAPIWrapper.Native.GPU.PerformanceStateId.P0_3DPerformance,
                new PerformanceStates20ClockEntryV1[]
                {
                    new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics, new PerformanceStates20ParameterDelta((int)vm.GpuClock * 1000)),
                    new PerformanceStates20ClockEntryV1(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory, new PerformanceStates20ParameterDelta((int)vm.MemoryClock * 1000))
                },
                new PerformanceStates20BaseVoltageEntryV1[] {}
            );

            var newInfo = new PerformanceStates20InfoV3(new PerformanceStates20InfoV1.PerformanceState20[] { newGpuPerfState }, 2, 0);

            GPUApi.SetPerformanceStates20(vm._gpu.Handle, newInfo);
            #endregion
        }
    }
}
