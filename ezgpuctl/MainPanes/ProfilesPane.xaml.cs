using GPUControl.Controls;
using GPUControl.Lib.GPU;
using GPUControl.Model;
using GPUControl.ViewModels;
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

namespace GPUControl.MainPanes
{
    /// <summary>
    /// Interaction logic for ProfilesMainPane.xaml
    /// </summary>
    public partial class ProfilesPane : UserControl
    {
        private List<IGpuWrapper> _gpus;
        private Settings _settings;
        private Action _reloadViewModel;
        private Action<Settings.OcModeType> _updateOcService;

        public ProfilesPane()
        {
            InitializeComponent();
        }

        public void DataInit(List<IGpuWrapper> gpus, Settings settings, Action reloadViewModel, Action<Settings.OcModeType> updateOcService)
        {
            _gpus = gpus;
            _settings = settings;
            _reloadViewModel = reloadViewModel;
            _updateOcService = updateOcService;
        }

        protected MainWindowViewModel ViewModel
        {
            get => (DataContext as MainWindowViewModel)!;
            set => DataContext = value;
        }

        private string FindAvailableName(string baseName, List<string> usedNames) =>
            Enumerable
                .Range(0, 1000)
                .Select(i => i == 0 ? baseName : $"{baseName} ({i})")
                .Where(l => !usedNames.Contains(l))
                .First();

        private bool IsProfileNameInUse(string name) => !ViewModel.Profiles.Any(p => p.Name == name);

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var newProfileName = FindAvailableName("New Profile", ViewModel.Settings.Profiles.Select(p => p.Name).ToList());
            var newProfile = new GpuOverclockProfile(newProfileName);
            var newProfileVm = new GpuOverclockProfileViewModel(_gpus, newProfile);
            var editorWindow = new OcProfileEditorWindow(newProfileName);
            editorWindow.ViewModel = newProfileVm;

            editorWindow.NewNameSelected += IsProfileNameInUse;

            if (editorWindow.ShowDialog() == true)
            {
                _settings.Profiles.Add(newProfileVm.AsModelObject);
                _settings.Save();

                _reloadViewModel();
            }

            editorWindow.NewNameSelected -= IsProfileNameInUse;
        }

        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!;
            var affectedPolicies = ViewModel.Policies.Where(pol => pol.Profiles.Contains(profile)).ToList();

            string detailsMsg = "";
            if (affectedPolicies.Count > 0)
            {
                detailsMsg = $" Removing this profile will affect {affectedPolicies.Count} policies.";
            }

            if (Xceed.Wpf.Toolkit.MessageBox.Show($"Are you sure you'd like to remove '{profile.Name}'?{detailsMsg}", "Remove Profile", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var modelProfile = _settings.Profiles.Single(p => p.Name == profile.Name);
                var modelPolicies = _settings.Policies.Where(p => affectedPolicies.Any(ap => ap.Name == p.Name)).ToList();
                foreach (var policy in modelPolicies)
                    policy.OrderedProfileNames.Remove(modelProfile.Name);

                if (_settings.OcMode_SpecificProfileName == profile.Name)
                    _settings.OcMode_SpecificProfileName = null;

                _settings.Profiles.Remove(modelProfile);
                _settings.Save();

                _reloadViewModel();
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!.AsModelObject;
            var editorWindow = new OcProfileEditorWindow(profile.Name);

            editorWindow.NewNameSelected += IsProfileNameInUse;
            editorWindow.ViewModel = new GpuOverclockProfileViewModel(_gpus, profile);

            if (editorWindow.ShowDialog() == true)
            {
                var oldProfileIndex = _settings.Profiles.FindIndex(p => p.Name == profile.Name);
                _settings.Profiles[oldProfileIndex] = editorWindow.ViewModel.AsModelObject;

                if (_settings.OcMode_SpecificProfileName == profile.Name)
                    _settings.OcMode_SpecificProfileName = editorWindow.ViewModel.Name;

                _settings.Save();

                _updateOcService(Settings.OcModeType.SpecificProfile);

                _reloadViewModel();
            }
        }
    }
}
