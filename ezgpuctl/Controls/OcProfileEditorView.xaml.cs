using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
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
    public class OcProfileEditorViewModel : ViewModel
    {
        public OcProfileEditorViewModel()
        {
            _label = "Sample Profile";
            _editorViewModels = new List<OcEditorViewModel>
            {
                new OcEditorViewModel { PowerTarget = 70 },
                new OcEditorViewModel { PowerTarget = 30, CanWrite = false }
            };
        }

        public OcProfileEditorViewModel(List<PhysicalGPU> gpus)
        {
            _label = "Unnamed Profile";
            _editorViewModels = gpus.Select(gpu => new OcEditorViewModel(gpu)).ToList();
        }

        public OcProfileEditorViewModel(List<PhysicalGPU> gpus, Model.GpuOverclockProfile baseProfile)
        {
            _label = baseProfile.Label;
            _editorViewModels = gpus.Select(gpu =>
            {
                var vm = new OcEditorViewModel(gpu);
                var existingOc = baseProfile.OverclockSettings.Where(s => s.GpuId == gpu.GPUId).FirstOrDefault();
                if (existingOc != null)
                {
                    if (existingOc.PowerTarget.HasValue) vm.PowerTarget = existingOc.PowerTarget.Value;
                    if (existingOc.CoreClockOffset.HasValue) vm.GpuClock = existingOc.CoreClockOffset.Value;
                    if (existingOc.MemoryClockOffset.HasValue) vm.MemoryClock = existingOc.MemoryClockOffset.Value;
                }
                if (baseProfile.IsReadOnly) vm.CanWrite = false;
                return vm;
            }).ToList();
        }

        private string _label;
        public string Label
        {
            get => _label;
            set
            {
                if (NewLabelSelected?.Invoke(value) == true)
                {
                    _label = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<OcEditorViewModel> _editorViewModels;
        public List<OcEditorViewModel> EditorViewModels
        {
            get => _editorViewModels;
            set
            {
                _editorViewModels = value;
                OnPropertyChanged();
            }
        }

        public event Func<string, bool>? NewLabelSelected;
    }

    /// <summary>
    /// Interaction logic for OcProfileEditorView.xaml
    /// </summary>
    public partial class OcProfileEditorView : UserControl
    {
        public OcProfileEditorView()
        {
            InitializeComponent();
        }
    }
}
