using GPUControl.Controls;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace GPUControl
{
    public class RealTimeGpuViewModel : ViewModel
    {
        public PhysicalGPU? _gpu;
        string _gpuName;
        public RealTimeGpuViewModel(PhysicalGPU? gpu)
        {
            this._gpu = gpu;
            _gpuStatus = new GpuStatusViewModel();

            if (_gpu != null)
            {
                var numDisplays = _gpu.ActiveOutputs.Length;
                _gpuName = $"{_gpu.FullName} ({numDisplays} displays)";
                Update();
            }
            else
            {
                _gpuName = "GPU Name";
                _gpuStatus.GpuName = _gpuName;
            }
        }

        private GpuStatusViewModel _gpuStatus;
        public GpuStatusViewModel GpuStatus
        {
            get => _gpuStatus;
            set
            {
                if (_gpuStatus != value)
                {
                    _gpuStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Update()
        {
            if (_gpu != null)
            {
                var gpuWrapper = new GpuWrapper(_gpu);

                GpuStatus = new GpuStatusViewModel
                {
                    GpuName = _gpuName,
                    CoreClock = gpuWrapper.Clocks.CurrentCoreClockMhz,
                    CoreBaseClock = gpuWrapper.Clocks.BaseCoreClockMhz,
                    MemoryClock = gpuWrapper.Clocks.CurrentMemoryClockMhz,
                    MemoryBaseClock = gpuWrapper.Clocks.BaseMemoryClockMhz,

                    CurrentPower = gpuWrapper.Power.CurrentPower,
                    PowerTarget = gpuWrapper.Power.CurrentTargetPower,
                    CurrentTemp = gpuWrapper.Temps.CurrentCoreTemp,
                    TempTarget = gpuWrapper.Temps.TempTarget,
                };
            }
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            GPUs = new List<RealTimeGpuViewModel> { new RealTimeGpuViewModel(null) };
            _selectedGpu = GPUs[0];
            //_selectedGpuOc = new OcEditorViewModel(_selectedGpu._gpu);
        }

        public MainWindowViewModel(List<PhysicalGPU> gpus)
        {
            GPUs = gpus.Select(gpu => new RealTimeGpuViewModel(gpu)).ToList();
            _selectedGpu = GPUs[0];
            _selectedGpuOc = new OcEditorViewModel(_selectedGpu._gpu);
        }

        public List<RealTimeGpuViewModel> GPUs { get; set; }

        private RealTimeGpuViewModel _selectedGpu;
        public RealTimeGpuViewModel SelectedGpu
        {
            get => _selectedGpu;
            set
            {
                if (_selectedGpu != value)
                {
                    _selectedGpu = value;
                    OnPropertyChanged();
                    SelectedGpuOc = new OcEditorViewModel(_selectedGpu._gpu);
                }
            }
        }

        private OcEditorViewModel _selectedGpuOc;
        public OcEditorViewModel SelectedGpuOc
        {
            get => _selectedGpuOc;
            set
            {
                if (_selectedGpuOc != value)
                {
                    _selectedGpuOc = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PhysicalGPU> _gpus;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            NVIDIA.Initialize();
            this._gpus = PhysicalGPU.GetPhysicalGPUs().ToList();
            this._viewModel = new MainWindowViewModel(_gpus);
            this.DataContext = this._viewModel;

            InitializeComponent();

            new DispatcherTimer(
                interval: TimeSpan.FromSeconds(1),
                priority: DispatcherPriority.Normal,
                callback: (sender, e) =>
                {
                    _viewModel.SelectedGpu.Update();
                },
                dispatcher: this.Dispatcher
            ).Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            NVIDIA.Unload();
        }
    }
}
