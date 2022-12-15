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

            _canWrite = true;
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

            _canWrite = true;
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

        private bool _canWrite;
        public bool CanWrite
        {
            get => _canWrite;
            set
            {
                _canWrite = value;
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

            
        }
    }
}
