using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPUControl.Controls;
using GPUControl.Lib.GPU;
using GPUControl.Model;
using GPUControl.Overclock;
using GPUControl.Util;
using GPUControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

using WinTS = Microsoft.Win32.TaskScheduler;

namespace GPUControl
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            GpuStatuses = new List<GpuStatusViewModel> { new GpuStatusViewModel(null) };
            selectedGpu = GpuStatuses[0];

            Settings = new SettingsViewModel();
        }

        public MainWindowViewModel(Dispatcher dispatcher, List<IGpuWrapper> gpus, Settings settings)
        {
            Settings = new SettingsViewModel(gpus, settings);
            PolicyService = new PolicyServiceViewModel(Settings, dispatcher);
            GpuStatuses = gpus.Select(gpu => new GpuStatusViewModel(gpu)).ToList();
            selectedGpu = GpuStatuses[0];

            Settings.Policies.CollectionChanged += Policies_CollectionChanged;

            AskBeforeClose = settings.AskBeforeClose;

            #region Policy Organization Commands
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
            #endregion

            Exit = new RelayCommand(() => ExitRequested?.Invoke());
        }

        private void Policies_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MovePolicyUp.NotifyCanExecuteChanged();
            MovePolicyDown.NotifyCanExecuteChanged();
            MovePolicyTop.NotifyCanExecuteChanged();
            MovePolicyBottom.NotifyCanExecuteChanged();
        }

        public SettingsViewModel Settings { get; private set; }

        public PolicyServiceViewModel PolicyService { get; }

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

        #region Settings

        public event Action ExitRequested;
        public IRelayCommand Exit { get; }

        public AutoStart RunOnStartup { get; } = new AutoStart();

        [ObservableProperty]
        private bool askBeforeClose;

        #endregion
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IGpuWrapper> _gpus;
        private Settings _settings;

        private List<string> _appliedProfileNames = new List<string>();
        private List<string> _appliedPolicyNames = new List<string>();

        public MainWindow()
        {
            IGpuWrapper.InitializeAll();

            if (IGpuWrapper.UseMockGpus) _settings = Settings.LoadFrom("settings-mock.json");
            else _settings = Settings.LoadFrom("settings.json");

            this._gpus = IGpuWrapper.ListAll();
            RefreshViewModel();

            ProcessMonitor.Start();
            OverclockManager.Start(_gpus);
            if (this._settings.PauseOcService) OverclockManager.Pause();
            else OverclockManager.Resume();

            OverclockManager.BehaviorApplied += OverclockManager_BehaviorApplied;

            ApplyOcMode(this._settings.OcMode, this._settings.OcMode switch
            {
                Settings.OcModeType.SpecificProfile => _settings.OcMode_SpecificProfileName,
                Settings.OcModeType.SpecificPolicy => _settings.OcMode_SpecificPolicyName,
                _ => null
            });

            InitializeComponent();

            if (File.Exists("dock.xml"))
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(DockManager);
                using (var reader = new StreamReader("dock.xml"))
                    layoutSerializer.Deserialize(reader);
            }

            PoliciesPane.DataInit(_gpus, _settings, RefreshViewModel, UpdateOcServiceSettings);
            ProfilesPane.DataInit(_gpus, _settings, RefreshViewModel, UpdateOcServiceSettings);
            SettingsPane.DataInit(_gpus, _settings, RefreshViewModel);


            //OverclockManager.PoliciesApplied += (policyNames) =>
            //{
            //    Dispatcher.BeginInvoke(() =>
            //    {
            //        foreach (var policy in ViewModel.Policies)
            //            policy.IsActive = policyNames.Contains(policy.Name);
            //    });
            //};
            
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

        private void ApplyOcSelectionStyles()
        {
            foreach (var profile in ViewModel.Profiles)
                profile.IsActive = _appliedProfileNames.Contains(profile.Name);

            foreach (var policy in ViewModel.Policies)
                policy.IsActive = _appliedPolicyNames.Contains(policy.Name);
        }

        private void OverclockManager_BehaviorApplied(Overclock.Result.IBehaviorResult obj)
        {
            Dispatcher.BeginInvoke(() =>
            {
                switch (obj)
                {
                    case Overclock.Result.ProfileResult profile:
                        _appliedProfileNames = new List<string>() { profile.ProfileName };
                        _appliedPolicyNames = new List<string>();
                        break;

                    case Overclock.Result.PoliciesResult policies:
                        _appliedProfileNames = (
                            from policyName in policies.AppliedPolicyNames
                            join policy in ViewModel.Policies on policyName equals policy.Name
                            from profile in policy.Profiles
                            select profile.Name
                        ).ToList();

                        _appliedPolicyNames = policies.AppliedPolicyNames;
                        break;
                }

                ApplyOcSelectionStyles();
            });
        }

        private void RefreshViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PolicyService.OcServiceStatusChanged -= OnOcServiceStatusChanged;
                ViewModel.PolicyService.OcModeChanged -= OnOcModeChanged;
                ViewModel.ExitRequested -= OnCloseByContextMenu;
            }

            var vm = new MainWindowViewModel(Dispatcher, _gpus, _settings);
            ViewModel = vm;
            vm.PolicyService.OcServiceStatusChanged += OnOcServiceStatusChanged;
            vm.PolicyService.OcModeChanged += OnOcModeChanged;
            vm.ExitRequested += OnCloseByContextMenu;

            ApplyOcSelectionStyles();
        }

        private void UpdateOcServiceSettings(Settings.OcModeType targetMode)
        {
            if (targetMode != ViewModel.Settings.OcMode) return;

            switch (targetMode)
            {
                case Settings.OcModeType.SpecificProfile:
                    OverclockManager.CurrentBehavior = new SpecificProfileOverclockBehavior(getViewModelSettings, _settings.OcMode_SpecificProfileName);
                    break;

                case Settings.OcModeType.SpecificPolicy:
                    OverclockManager.CurrentBehavior = new SpecificPolicyOverclockBehavior(getViewModelSettings, _settings.OcMode_SpecificPolicyName);
                    break;
            }
        }

        private async Task<Settings> getViewModelSettings()
        {
            SettingsViewModel? result = null;
            await Dispatcher.BeginInvoke(() => result = ViewModel.Settings).Task;
            return result!.AsDisplayModelObject;
        }

        private void ApplyOcMode(Settings.OcModeType targetMode, string? name)
        {
            switch (targetMode)
            {
                case Settings.OcModeType.Stock:
                    _settings.OcMode = Settings.OcModeType.Stock;
                    OverclockManager.CurrentBehavior = new StockOverclockBehavior();
                    break;

                case Settings.OcModeType.Policies:
                    _settings.OcMode = Settings.OcModeType.Policies;
                    OverclockManager.CurrentBehavior = new MultiPolicyBehavior(getViewModelSettings);
                    break;

                case Settings.OcModeType.SpecificPolicy:
                    _settings.OcMode = Settings.OcModeType.SpecificPolicy;
                    _settings.OcMode_SpecificPolicyName = name;
                    OverclockManager.CurrentBehavior = new SpecificPolicyOverclockBehavior(getViewModelSettings, name);
                    break;

                case Settings.OcModeType.SpecificProfile:
                    _settings.OcMode = Settings.OcModeType.SpecificProfile;
                    _settings.OcMode_SpecificProfileName = name;
                    OverclockManager.CurrentBehavior = new SpecificProfileOverclockBehavior(getViewModelSettings, name);
                    break;
            }
        }

        // TODO - Invoke on profile / policy name change to propagate new name
        //        to OC service
        private void OnOcModeChanged(Settings.OcModeType newMode, string? name)
        {
            ApplyOcMode(newMode, name);

            _settings.Save();
            RefreshViewModel();
        }

        private void OnOcServiceStatusChanged(bool shouldResume)
        {
            _settings.PauseOcService = !shouldResume;
            _settings.Save();

            if (shouldResume) OverclockManager.Resume();
            else OverclockManager.Pause();

            RefreshViewModel();
        }

        public MainWindowViewModel ViewModel
        {
            get => (DataContext as MainWindowViewModel)!;
            set => DataContext = value;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            string closeMessage = "Are you sure you'd like to exit? GPU Control will no longer be applying OC settings.";
            if (_settings.AskBeforeClose && Xceed.Wpf.Toolkit.MessageBox.Show(closeMessage, "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(DockManager);
            using (var writer = new StreamWriter("dock.xml"))
                layoutSerializer.Serialize(writer);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            OverclockManager.Stop();
            ProcessMonitor.Stop();
            IGpuWrapper.UnloadAll();
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                Activate();
            }
            else
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void OnCloseByContextMenu()
        {
            this.Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized) ShowInTaskbar = false;
            else ShowInTaskbar = true;
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Activate();
        }
    }
}
