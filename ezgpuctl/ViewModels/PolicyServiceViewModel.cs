using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GPUControl.Model;
using GPUControl.Overclock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GPUControl.ViewModels
{
    public partial class PolicyServiceViewModel : ObservableObject
    {
        private Dispatcher updateDispatcher;
        private SettingsViewModel parent;

        public PolicyServiceViewModel(SettingsViewModel parent, Dispatcher updateDispatcher)
        {
            this.updateDispatcher = updateDispatcher;
            this.parent = parent;

            //OverclockManager.PoliciesApplied += (names) =>
            //{
            //    updateDispatcher.BeginInvoke(() => AppliedProfileNames = new ObservableCollection<string>(names));
            //};

            OverclockManager.ManagerStateChanged += () => IsRunning = OverclockManager.IsRunning;

            StartOcService = new RelayCommand(
                () =>
                {
                    OcServiceStatusChanged?.Invoke(true);
                    StartOcService!.NotifyCanExecuteChanged();
                    StopOcService!.NotifyCanExecuteChanged();
                },
                () => !OverclockManager.IsRunning
            );

            StopOcService = new RelayCommand(
                () =>
                {
                    OcServiceStatusChanged?.Invoke(false);
                    StartOcService!.NotifyCanExecuteChanged();
                    StopOcService!.NotifyCanExecuteChanged();
                },
                () => OverclockManager.IsRunning
            );

            SetOcModePolicies = new RelayCommand(
                () => OcModeChanged?.Invoke(Model.Settings.OcModeType.Policies, null)
            );

            SetOcModeSpecificPolicy = new RelayCommand(
                () => OcModeChanged?.Invoke(Model.Settings.OcModeType.SpecificPolicy, parent.OcMode_SpecificPolicy?.Name)
            );

            SetOcModeSpecificProfile = new RelayCommand(
                () => OcModeChanged?.Invoke(Model.Settings.OcModeType.SpecificProfile, parent.OcMode_SpecificProfile?.Name)
            );

            SetOcModeStock = new RelayCommand(
                () => OcModeChanged?.Invoke(Model.Settings.OcModeType.Stock, null)
            );

            SetOcModeSpecificPolicyName = new RelayCommand<string>(
                (name) => OcModeChanged?.Invoke(Model.Settings.OcModeType.SpecificPolicy, name)
            );

            SetOcModeSpecificProfileName = new RelayCommand<string>(
                (name) => OcModeChanged?.Invoke(Model.Settings.OcModeType.SpecificProfile, name)
            );
        }

        [ObservableProperty]
        private ObservableCollection<string> appliedProfileNames = new ObservableCollection<string>();

        [ObservableProperty]
        private bool isRunning = false;

        // bool = should resume
        public event Action<bool> OcServiceStatusChanged;

        public event Action<Model.Settings.OcModeType, string?> OcModeChanged;

        
        public IRelayCommand StartOcService { get; }
        public IRelayCommand StopOcService { get; }


        public bool IsOcModeStock => parent.OcMode == Model.Settings.OcModeType.Stock;
        public bool IsOcModePolicies => parent.OcMode == Model.Settings.OcModeType.Policies;
        public bool IsOcModeSpecificPolicy => parent.OcMode == Model.Settings.OcModeType.SpecificPolicy;
        public bool IsOcModeSpecificProfile => parent.OcMode == Model.Settings.OcModeType.SpecificProfile;

        public IRelayCommand SetOcModeStock { get; }
        public IRelayCommand SetOcModePolicies { get; }
        public IRelayCommand SetOcModeSpecificPolicy { get; }
        public IRelayCommand SetOcModeSpecificProfile { get; }

        public IRelayCommand SetOcModeSpecificPolicyName { get; }
        public IRelayCommand SetOcModeSpecificProfileName { get; }

        public class AvailableSpecificItem
        {
            public string Name { get; set; }
            public bool IsInUse { get; set; }

            public IRelayCommand Select { get; set; }
        }

        public ObservableCollection<AvailableSpecificItem> PolicyApplications
        {
            get
            {
                var selectedPolicyName = (OverclockManager.CurrentBehavior as SpecificPolicyOverclockBehavior)?.PolicyName;
                return new ObservableCollection<AvailableSpecificItem>(parent.Policies.Select(p =>
                    new AvailableSpecificItem { Name = p.Name, IsInUse = p.Name == selectedPolicyName, Select = SetOcModeSpecificPolicyName }
                ));
            }
        }

        public ObservableCollection<AvailableSpecificItem> ProfileApplications
        {
            get
            {
                var selectedProfileName = (OverclockManager.CurrentBehavior as SpecificProfileOverclockBehavior)?.ProfileName;
                return new ObservableCollection<AvailableSpecificItem>(parent.Profiles.Select(p =>
                    new AvailableSpecificItem { Name = p.Name, IsInUse = p.Name == selectedProfileName, Select = SetOcModeSpecificProfileName }
                ));
            }
        }
    }
}
