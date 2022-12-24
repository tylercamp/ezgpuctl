using GPUControl.Controls;
using GPUControl.Lib.GPU;
using GPUControl.Model;
using GPUControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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

namespace GPUControl.MainPanes
{
    /// <summary>
    /// Interaction logic for PolicyEditorMainPane.xaml
    /// </summary>
    public partial class PoliciesPane : UserControl
    {
        private List<IGpuWrapper> _gpus;
        private Settings _settings;
        private Action _reloadViewModel;
        private Action<Settings.OcModeType> _updateOcService;

        public PoliciesPane()
        {
            InitializeComponent();
        }

        protected MainWindowViewModel ViewModel
        {
            get => (DataContext as MainWindowViewModel)!;
            set => DataContext = value;
        }

        public void DataInit(List<IGpuWrapper> gpus, Settings settings, Action reloadViewModel, Action<Settings.OcModeType> updateOcService)
        {
            _gpus = gpus;
            _settings = settings;
            _reloadViewModel = reloadViewModel;
            _updateOcService = updateOcService;
        }

        private bool IsPolicyNameInUse(string name) => !ViewModel.Policies.Any(p => p.Name == name);

        private string FindAvailableName(string baseName, List<string> usedNames) =>
            Enumerable
                .Range(0, 1000)
                .Select(i => i == 0 ? baseName : $"{baseName} ({i})")
                .Where(l => !usedNames.Contains(l))
                .First();

        private void AddPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            var newPolicyName = FindAvailableName("New Policy", ViewModel.Policies.Select(p => p.Name).ToList());

            var newPolicy = new GpuOverclockPolicy(newPolicyName);
            var newPolicyVm = new GpuOverclockPolicyViewModel(ViewModel.Settings, newPolicy);
            var editorWindow = new OcPolicyEditorWindow(newPolicyName);

            editorWindow.ViewModel = new OcPolicyEditorWindowViewModel(newPolicyVm);
            editorWindow.NewNameSelected += IsPolicyNameInUse;

            if (editorWindow.ShowDialog() == true)
            {
                _settings.Policies.Add(newPolicyVm.AsModelObject);
                _settings.Save();

                _reloadViewModel();
            }

            editorWindow.NewNameSelected -= IsPolicyNameInUse;
        }

        private void RemovePolicyButton_Click(object sender, RoutedEventArgs e)
        {
            var policy = ViewModel.SelectedPolicy!;
            if (Xceed.Wpf.Toolkit.MessageBox.Show($"Are you sure you'd like to remove the policy '{policy.Name}'?", "Remove Policy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var modelPolicy = _settings.Policies.Single(p => p.Name == policy.Name);
                _settings.Policies.Remove(modelPolicy);

                if (_settings.OcMode_SpecificPolicyName == modelPolicy.Name)
                    _settings.OcMode_SpecificPolicyName = null;

                _settings.Save();

                _reloadViewModel();
            }
        }

        private void EditPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            var modelPolicy = ViewModel.SelectedPolicy!.AsModelObject;
            var editorWindow = new OcPolicyEditorWindow(modelPolicy.Name);

            editorWindow.ViewModel = new OcPolicyEditorWindowViewModel(new GpuOverclockPolicyViewModel(ViewModel.Settings, modelPolicy));
            editorWindow.NewNameSelected += IsPolicyNameInUse;

            if (editorWindow.ShowDialog() == true)
            {
                var oldPolicyIndex = _settings.Policies.FindIndex(p => p.Name == modelPolicy.Name);
                _settings.Policies[oldPolicyIndex] = editorWindow.ViewModel.Policy.AsModelObject;
                if (_settings.OcMode_SpecificPolicyName == modelPolicy.Name)
                    _settings.OcMode_SpecificPolicyName = editorWindow.ViewModel.Policy.Name;
                _settings.Save();

                _updateOcService(Settings.OcModeType.SpecificPolicy);
                _reloadViewModel();
            }

            editorWindow.NewNameSelected -= IsPolicyNameInUse;
        }
    }
}
