using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPUControl.Controls;
using GPUControl.Model;
using GPUControl.ViewModels;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            GpuStatuses = new List<GpuStatusViewModel> { new GpuStatusViewModel(null) };
            selectedGpu = GpuStatuses[0];
        }

        public MainWindowViewModel(List<PhysicalGPU> gpus, Settings settings)
        {
            Settings = new SettingsViewModel(gpus, settings);
            GpuStatuses = gpus.Select(gpu => new GpuStatusViewModel(gpu)).ToList();
            selectedGpu = GpuStatuses[0];

            Settings.Policies.CollectionChanged += Policies_CollectionChanged;

            MovePolicyUp = new RelayCommand(
                () =>
                {
                    var idx = Policies.IndexOf(SelectedPolicy);
                    Policies.Move(idx, idx - 1);

                    var modelCurrent = settings.Policies[idx];
                    var modelOther = settings.Policies[idx - 1];

                    settings.Policies[idx] = modelOther;
                    settings.Policies[idx - 1] = modelCurrent;
                    settings.Save();
                },
                () => SelectedPolicy?.IsReadOnly == false && Policies.IndexOf(SelectedPolicy) > 0
            );

            MovePolicyDown = new RelayCommand(
                () =>
                {
                    var idx = Policies.IndexOf(SelectedPolicy);
                    Policies.Move(idx, idx + 1);

                    var modelCurrent = settings.Policies[idx];
                    var modelOther = settings.Policies[idx + 1];

                    settings.Policies[idx] = modelOther;
                    settings.Policies[idx + 1] = modelCurrent;
                    settings.Save();
                },
                () => SelectedPolicy?.IsReadOnly == false && Policies.IndexOf(SelectedPolicy) < Policies.Count - 2
            );

            MovePolicyTop = new RelayCommand(
                () =>
                {
                    var idx = Policies.IndexOf(SelectedPolicy);

                    Policies.Move(idx, 0);

                    var modelCurrent = settings.Policies[idx];
                    settings.Policies.RemoveAt(idx);
                    settings.Policies.Insert(0, modelCurrent);
                    settings.Save();
                },
                () => SelectedPolicy?.IsReadOnly == false && Policies.IndexOf(SelectedPolicy) != 0
            );

            MovePolicyBottom = new RelayCommand(
                () =>
                {
                    var idx = Policies.IndexOf(SelectedPolicy);
                    Policies.Move(idx, Policies.Count - 2);

                    var modelCurrent = settings.Policies[idx];
                    settings.Policies.RemoveAt(idx);
                    settings.Policies.Add(modelCurrent);
                    settings.Save();
                },
                () => SelectedPolicy?.IsReadOnly == false && Policies.IndexOf(SelectedPolicy) < Policies.Count - 2
            );
        }

        private void Policies_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MovePolicyUp.NotifyCanExecuteChanged();
            MovePolicyDown.NotifyCanExecuteChanged();
            MovePolicyTop.NotifyCanExecuteChanged();
            MovePolicyBottom.NotifyCanExecuteChanged();
        }

        public SettingsViewModel Settings { get; private set; }

        public List<GpuStatusViewModel> GpuStatuses { get; }

        public ObservableCollection<GpuOverclockProfileViewModel> Profiles => Settings.Profiles;
        public ObservableCollection<GpuOverclockPolicyViewModel> Policies => Settings.Policies;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanEditProfile))]
        [NotifyPropertyChangedFor(nameof(CanRemoveProfile))]
        private GpuOverclockProfileViewModel selectedProfile;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanEditPolicy))]
        [NotifyPropertyChangedFor(nameof(CanRemovePolicy))]
        [NotifyCanExecuteChangedFor(nameof(MovePolicyUp))]
        [NotifyCanExecuteChangedFor(nameof(MovePolicyDown))]
        [NotifyCanExecuteChangedFor(nameof(MovePolicyTop))]
        [NotifyCanExecuteChangedFor(nameof(MovePolicyBottom))]
        private GpuOverclockPolicyViewModel selectedPolicy;

        [ObservableProperty]
        private GpuStatusViewModel selectedGpu;

        public IRelayCommand MovePolicyUp { get; }
        public IRelayCommand MovePolicyDown { get; }
        public IRelayCommand MovePolicyTop { get; }
        public IRelayCommand MovePolicyBottom { get; }

        public bool CanEditProfile => SelectedProfile?.IsReadOnly == false;
        public bool CanRemoveProfile => SelectedProfile?.IsReadOnly == false;

        public bool CanEditPolicy => SelectedPolicy?.IsReadOnly == false;
        public bool CanRemovePolicy => SelectedPolicy?.IsReadOnly == false;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PhysicalGPU> _gpus;
        private Settings _settings;

        public MainWindow()
        {
            ProcessMonitor.Start();

            NVIDIA.Initialize();
            _settings = Settings.LoadFrom("settings.json");

            this._gpus = PhysicalGPU.GetPhysicalGPUs().ToList();
            ViewModel = new MainWindowViewModel(_gpus, _settings);

            InitializeComponent();

            KeyDown += (s, e) =>
            {
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) _settings.Save();
            };

            var defaultPolicy = ViewModel.Policies.Where(p => p.IsReadOnly).Single();
            var defaultProfile = ViewModel.Profiles.Where(p => p.IsReadOnly).Single();
            PolicyMonitor.Start(_settings, defaultPolicy.AsModelObject, defaultProfile.AsModelObject, _gpus);

            PolicyMonitor.PoliciesApplied += (policyNames) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    foreach (var policy in ViewModel.Policies)
                        policy.IsActive = policyNames.Contains(policy.Name);
                });
            };
            
            // refresh GPU status view
            new DispatcherTimer(
                interval: TimeSpan.FromSeconds(1),
                priority: DispatcherPriority.Normal,
                callback: (sender, e) =>
                {
                    ViewModel.SelectedGpu.UpdateState();
                },
                dispatcher: this.Dispatcher
            ).Start();
        }

        public MainWindowViewModel ViewModel
        {
            get => (DataContext as MainWindowViewModel)!;
            set => DataContext = value;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            PolicyMonitor.Stop();
            ProcessMonitor.Stop();
            NVIDIA.Unload();
        }

        private bool IsProfileNameInUse(string name) => !ViewModel.Profiles.Any(p => p.Name == name);
        private bool IsPolicyNameInUse(string name) => !ViewModel.Policies.Any(p => p.Name == name);

        private string FindAvailableName(string baseName, List<string> usedNames) =>
            Enumerable
                .Range(0, 1000)
                .Select(i => i == 0 ? baseName : $"{baseName} ({i})")
                .Where(l => !usedNames.Contains(l))
                .First();

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

                ViewModel = new MainWindowViewModel(_gpus, _settings);
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

            if (MessageBox.Show($"Are you sure you'd like to remove '{profile.Name}'?{detailsMsg}", "Remove Profile", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var modelProfile = _settings.Profiles.Single(p => p.Name == profile.Name);
                var modelPolicies = _settings.Policies.Where(p => affectedPolicies.Any(ap => ap.Name == p.Name)).ToList();
                foreach (var policy in modelPolicies)
                    policy.OrderedProfileNames.Remove(modelProfile.Name);

                _settings.Profiles.Remove(modelProfile);
                _settings.Save();

                ViewModel = new MainWindowViewModel(_gpus, _settings);
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
                _settings.Profiles[oldProfileIndex] = profile;
                _settings.Save();

                ViewModel = new MainWindowViewModel(_gpus, _settings);
            }
        }

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

                ViewModel = new MainWindowViewModel(_gpus, _settings);
            }

            editorWindow.NewNameSelected -= IsPolicyNameInUse;
        }

        private void RemovePolicyButton_Click(object sender, RoutedEventArgs e)
        {
            var policy = ViewModel.SelectedPolicy!;
            if (MessageBox.Show($"Are you sure you'd like to remove the policy '{policy.Name}'?", "Remove Policy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var modelPolicy = _settings.Policies.Single(p => p.Name == policy.Name);
                _settings.Policies.Remove(modelPolicy);
                _settings.Save();

                ViewModel = new MainWindowViewModel(_gpus, _settings);
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
                _settings.Save();

                ViewModel = new MainWindowViewModel(_gpus, _settings);
            }

            editorWindow.NewNameSelected -= IsPolicyNameInUse;
        }
    }
}
