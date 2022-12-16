﻿using GPUControl.Controls;
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
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            GpuStatuses = new List<GpuStatusViewModel> { new GpuStatusViewModel(null) };
            _selectedGpu = GpuStatuses[0];
        }

        public MainWindowViewModel(List<PhysicalGPU> gpus, Settings settings)
        {
            Settings = new SettingsViewModel(gpus, settings);
            GpuStatuses = gpus.Select(gpu => new GpuStatusViewModel(gpu)).ToList();
            _selectedGpu = GpuStatuses[0];
        }

        public SettingsViewModel Settings { get; private set; }

        public List<GpuStatusViewModel> GpuStatuses { get; }

        public ObservableCollection<GpuOverclockProfileViewModel> Profiles => Settings.Profiles;
        public ObservableCollection<GpuOverclockPolicyViewModel> Policies => Settings.Policies;

        private GpuOverclockProfileViewModel _selectedProfile;
        public GpuOverclockProfileViewModel SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                _selectedProfile = value;
                OnPropertyChanged();
            }
        }

        private GpuOverclockPolicyViewModel _selectedPolicy;
        public GpuOverclockPolicyViewModel SelectedPolicy
        {
            get => _selectedPolicy;
            set
            {
                _selectedPolicy = value;
                OnPropertyChanged();
            }
        }

        private GpuStatusViewModel _selectedGpu;
        public GpuStatusViewModel SelectedGpu
        {
            get => _selectedGpu;
            set
            {
                if (_selectedGpu != value)
                {
                    _selectedGpu = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PhysicalGPU> _gpus;
        private MainWindowViewModel _viewModel;
        private Settings _settings;

        public MainWindow()
        {
            NVIDIA.Initialize();
            _settings = Settings.LoadFrom("settings.json");

            this._gpus = PhysicalGPU.GetPhysicalGPUs().ToList();
            this._viewModel = new MainWindowViewModel(_gpus, _settings);
            this.DataContext = this._viewModel;

            InitializeComponent();

            KeyDown += (s, e) =>
            {
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) _settings.Save();
            };
            
            // refresh GPU status view
            new DispatcherTimer(
                interval: TimeSpan.FromSeconds(1),
                priority: DispatcherPriority.Normal,
                callback: (sender, e) =>
                {
                    _viewModel.SelectedGpu.UpdateState();
                },
                dispatcher: this.Dispatcher
            ).Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            NVIDIA.Unload();
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var newProfile = new GpuOverclockProfile("New Profile");
            var newProfileVm = new GpuOverclockProfileViewModel(_gpus, newProfile);
            var editorWindow = new OcProfileEditorWindow();
            editorWindow.DataContext = newProfileVm;
            
            editorWindow.NewNameSelected += name => _settings.Profiles.Any(p => p.Label == name);

            if (editorWindow.ShowDialog() == true)
            {
                newProfileVm.ApplyPendingLabel();
                newProfileVm.ApplyChanges();
                _viewModel.Settings.AddProfile(newProfile, newProfileVm);
            }
        }
    }
}
