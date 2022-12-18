using GPUControl.Model;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public class GpuOverclockPolicyViewModel : ViewModel
    {
        GpuOverclockPolicy policy;
        SettingsViewModel? parent;

        // for XAML design view only!
        public GpuOverclockPolicyViewModel()
        {
            _pendingName = "New Policy";
            parent = null;

            _profiles = new ObservableCollection<GpuOverclockProfileViewModel>();
            _profiles.Add(new GpuOverclockProfileViewModel());

            _rules = new ObservableCollection<ProgramPolicyRuleViewModel>();
            _rules .Add(new ProgramPolicyRuleViewModel(new ProgramPolicyRule()
            {
                Negated = true,
                ProgramName = "program.exe"
            }));

            _pendingProfiles = _profiles;
            _pendingRules = _rules;
        }

        // for normal use
        public GpuOverclockPolicyViewModel(SettingsViewModel? parent, GpuOverclockPolicy policy)
        {
            this.parent = parent;
            this.policy = policy;

            _pendingName = policy.Name;

            IsReadOnly = false;

            _profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                from profileName in policy.OrderedProfileNames
                join profileVm in parent.Profiles
                on profileName equals profileVm.Name
                select profileVm
            );

            _rules = new ObservableCollection<ProgramPolicyRuleViewModel>(policy.Rules.Select(r => new ProgramPolicyRuleViewModel(r)));

            _pendingProfiles = new ObservableCollection<GpuOverclockProfileViewModel>(Profiles);
            _pendingRules = new ObservableCollection<ProgramPolicyRuleViewModel>(Rules);
        }


        // for default policy
        private GpuOverclockPolicyViewModel(GpuOverclockPolicy policy, IEnumerable<GpuOverclockProfileViewModel> profileVms)
        {
            this.parent = null;
            this.policy = policy;

            IsReadOnly = true;

            _pendingName = policy.Name;

            _profiles = new ObservableCollection<GpuOverclockProfileViewModel>(profileVms);
            _rules = new ObservableCollection<ProgramPolicyRuleViewModel>();

            _pendingProfiles = Profiles;
            _pendingRules = Rules;
        }

        public static GpuOverclockPolicyViewModel GetDefault(GpuOverclockProfileViewModel defaultProfile)
        {
            var policy = new Model.GpuOverclockPolicy("Default")
            {
                OrderedProfileNames = new List<string>() { defaultProfile.Name },
                Rules = new List<Model.ProgramPolicyRule>(),
            };

            return new GpuOverclockPolicyViewModel(policy, new List<GpuOverclockProfileViewModel>() { defaultProfile });
        }

        public bool IsReadOnly { get; private set; }

        private string _pendingName;
        public string Name
        {
            get => _pendingName;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                _pendingName = value;
                OnPropertyChanged();
            }
        }

        public List<GpuOverclockProfileViewModel> AvailableProfiles => parent?.Profiles?.Where(p => !Profiles.Contains(p))?.ToList() ?? new List<GpuOverclockProfileViewModel>();

        private ObservableCollection<GpuOverclockProfileViewModel> _profiles;
        public ObservableCollection<GpuOverclockProfileViewModel> Profiles
        {
            get => _profiles;
            set
            {
                if (_profiles != value)
                {
                    _profiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ProgramPolicyRuleViewModel> _rules;
        public ObservableCollection<ProgramPolicyRuleViewModel> Rules
        {
            get => _rules;
            set
            {
                if (_rules != value)
                {
                    _rules = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<GpuOverclockProfileViewModel> _pendingProfiles;
        public ObservableCollection<GpuOverclockProfileViewModel> PendingProfiles
        {
            get => _pendingProfiles;
            set
            {
                _pendingProfiles = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ProgramPolicyRuleViewModel> _pendingRules;
        public ObservableCollection<ProgramPolicyRuleViewModel> PendingRules
        {
            get => _pendingRules;
            set
            {
                _pendingRules = value;
                OnPropertyChanged();
            }
        }

        public GpuOverclockPolicy ModelPolicy => policy;

        // TODO - Attach to HasPendingChanges property event for sub-objects and raise change for this VM's HasPendingChanges
        // TODO - Ability to revert changes to the list of profiles / rules

        public bool HasPendingChanges => Profiles.Any(p => p.HasChanges) || Rules.Any(r => r.HasChanges);

        public void ApplyPendingChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            policy.Name = _pendingName;

            policy.Rules = Rules.Select(vm => new ProgramPolicyRule { ProgramName = vm.ProgramName, Negated = vm.Negated }).ToList();
            policy.OrderedProfileNames = Profiles.Select(vm => vm.Name).ToList();

            Profiles = PendingProfiles;
            Rules = PendingRules;
        }

        public void RevertPendingChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            Name = policy.Name;
            PendingProfiles = Profiles;
            PendingRules = Rules;
        }
    }
}
