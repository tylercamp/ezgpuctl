using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPUControl.Controls;
using GPUControl.Lib.GPU;
using GPUControl.Model;
using GPUControl.Overclock;
using GPUControl.Util;
using GPUControl.ViewModels;
using Serilog;
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
            StartMinimized = settings.HideOnStartup;

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

            ToggleRunOnStartup = new RelayCommand(() =>
            {
                RunOnStartup.IsEnabled = !RunOnStartup.IsEnabled;
            });

            ToggleStartMinimized = new RelayCommand(() =>
            {
                StartMinimized = !StartMinimized;
                SettingsDisplayChanged?.Invoke();
            });

            ToggleAskBeforeClose = new RelayCommand(() =>
            {
                AskBeforeClose = !AskBeforeClose;
                SettingsDisplayChanged?.Invoke();
            });

            ShowAboutWindow = new RelayCommand(() => new AboutWindow().ShowDialog());
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

        [ObservableProperty]
        private string? ocStatusSummary;

        #region Settings

        public event Action SettingsDisplayChanged;

        public event Action ExitRequested;
        public IRelayCommand Exit { get; }

        public IRelayCommand ToggleRunOnStartup { get; }
        public IRelayCommand ToggleStartMinimized { get; }
        public IRelayCommand ToggleAskBeforeClose { get; }

        public IRelayCommand ShowAboutWindow { get; }

        public AutoStart RunOnStartup { get; } = new AutoStart();

        [ObservableProperty]
        private bool startMinimized;

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

        private ILogger logger = Log.ForContext<MainWindow>();

        public MainWindow()
        {
            bool initialized = IGpuWrapper.InitializeAll((type, ex) =>
            {
                logger.Warning(ex, "Error initializing {0}", type.FullName);
            });

            if (!initialized)
            {
                MessageBox.Show("Unable to initialize. See logs for more info.");
                this.Close();
                return;
            }

            if (IGpuWrapper.UseMockGpus) _settings = Settings.LoadFrom("settings-mock.json");
            else _settings = Settings.LoadFrom("settings.json");

            if (_settings.IsFirstRun)
            {
                string warningMessage =
                    "Overclocking can permanently damage your hardware. " +
                    "The author(s) of this app and its dependencies hold no responsibility or liability for any damages resulting from its use." +
                    "\n\n" +
                    "Do you agree to these terms?";

                if (MessageBox.Show(warningMessage, "Warning!", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    Environment.Exit(1);
                }

                MessageBox.Show("Settings for this app can be found by right-clicking its icon in the System Tray.", "Notice");
            }

            _settings.Save();

            this._gpus = IGpuWrapper.ListAll();
            logger.Information("Found {0} GPUs", _gpus.Count);

            RefreshViewModel();

            OverclockManager.BehaviorApplied += OverclockManager_BehaviorApplied;
            OverclockManager.UnexpectedError += OverclockManager_UnexpectedError;

            ProcessMonitor.Start();
            OverclockManager.Start(_gpus);
            if (this._settings.PauseOcService) OverclockManager.Pause();
            else OverclockManager.Resume();

            ApplyOcMode(this._settings.OcMode, this._settings.OcMode switch
            {
                Settings.OcModeType.SpecificProfile => _settings.OcMode_SpecificProfileName,
                Settings.OcModeType.SpecificPolicy => _settings.OcMode_SpecificPolicyName,
                _ => null
            });

            InitializeComponent();

            if (File.Exists("dock.xml"))
            {
                try
                {
                    XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(DockManager);
                    using (var reader = new StreamReader("dock.xml"))
                        layoutSerializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    logger.Warning(ex, "An error occurred while loading previous dock state, resetting");
                    File.Delete("dock.xml");
                }
            }

            // yuck
            PoliciesPane.DataInit(_gpus, _settings, RefreshViewModel, UpdateOcServiceSettings);
            ProfilesPane.DataInit(_gpus, _settings, RefreshViewModel, UpdateOcServiceSettings);
            
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

            if (_settings.HideOnStartup)
            {
                new DispatcherTimer(
                    interval: TimeSpan.FromSeconds(1),
                    priority: DispatcherPriority.Normal,
                    callback: (sender, e) =>
                    {
                        WindowState = WindowState.Minimized;
                        ShowInTaskbar = false;

                        var timer = (sender as DispatcherTimer)!;
                        timer.Stop();
                    },
                    dispatcher: this.Dispatcher
                ).Start();
            }
        }

        private void OverclockManager_UnexpectedError(Exception obj)
        {
            logger.Error(obj, "Exception while applying overclocks");
            MessageBox.Show("An error occurred while applying overclocks. OC service has been paused.");
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

                ViewModel.OcStatusSummary = BuildOcStatusSummary();
                ApplyOcSelectionStyles();
            });
        }

        private void RefreshViewModel()
        {
            logger.Verbose("Refreshing MainWindowViewModel");

            if (ViewModel != null)
            {
                ViewModel.PolicyService.OcServiceStatusChanged -= OnOcServiceStatusChanged;
                ViewModel.PolicyService.OcModeChanged -= OnOcModeChanged;
                ViewModel.ExitRequested -= OnCloseByContextMenu;
                ViewModel.SettingsDisplayChanged -= OnSettingsDisplayChanged;

                ViewModel.PolicyService.Dispose();
            }

            var vm = new MainWindowViewModel(Dispatcher, _gpus, _settings);
            ViewModel = vm;
            vm.PolicyService.OcServiceStatusChanged += OnOcServiceStatusChanged;
            vm.PolicyService.OcModeChanged += OnOcModeChanged;
            vm.ExitRequested += OnCloseByContextMenu;
            vm.SettingsDisplayChanged += OnSettingsDisplayChanged;
            vm.OcStatusSummary = BuildOcStatusSummary();
            ApplyOcSelectionStyles();
        }

        private void OnSettingsDisplayChanged()
        {
            _settings.AskBeforeClose = ViewModel.AskBeforeClose;
            _settings.HideOnStartup = ViewModel.StartMinimized;
            _settings.Save();
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

        private void OnOcModeChanged(Settings.OcModeType newMode, string? name)
        {
            ApplyOcMode(newMode, name);

            _settings.Save();
            RefreshViewModel();
        }

        private string BuildOcStatusSummary()
        {
            if (!OverclockManager.IsRunning) return "OC service is not running - OCs are not being applied.";
            else if (OverclockManager.CurrentBehavior == null) return "OC service is running but no behavior has been selected."; // shouldn't happen
            else if (OverclockManager.LastResult == null) return "OC service is running but is still starting up.";

            var resultLines = new List<string>();

            string ocMode = OverclockManager.CurrentBehavior switch
            {
                MultiPolicyBehavior => "Policies",
                StockOverclockBehavior => "Stock",
                SpecificPolicyOverclockBehavior => "Specific Policy",
                SpecificProfileOverclockBehavior => "Specific Profile",
                _ => "Unknown"
            };

            resultLines.Add($"Mode: {ocMode}");

            resultLines.Add("");

            foreach (var oc in OverclockManager.LastResult.AppliedOverclocks)
            {
                resultLines.Add(oc.ToString());
                resultLines.Add("");
            }

            return string.Join("\n", resultLines);
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

            if (ViewModel == null) return;

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
