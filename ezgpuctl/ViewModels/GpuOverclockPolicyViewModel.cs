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

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>();
            Profiles.Add(new GpuOverclockProfileViewModel());

            Rules = new ObservableCollection<ProgramPolicyRuleViewModel>();
            Rules.Add(new ProgramPolicyRuleViewModel(new ProgramPolicyRule()
            {
                Negated = true,
                ProgramName = "program.exe"
            }));
        }

        // for normal use
        public GpuOverclockPolicyViewModel(SettingsViewModel? parent, GpuOverclockPolicy policy)
        {
            this.parent = parent;
            this.policy = policy;

            _pendingName = policy.Name;

            IsReadOnly = false;

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                from profileName in policy.OrderedProfileNames
                join profileVm in parent.Profiles
                on profileName equals profileVm.Name
                select profileVm
            );

            Rules = new ObservableCollection<ProgramPolicyRuleViewModel>(policy.Rules.Select(r => new ProgramPolicyRuleViewModel(r)));
        }


        // for default policy
        private GpuOverclockPolicyViewModel(GpuOverclockPolicy policy, IEnumerable<GpuOverclockProfileViewModel> profileVms)
        {
            this.parent = null;
            this.policy = policy;

            IsReadOnly = true;

            _pendingName = policy.Name;

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(profileVms);
            Rules = new ObservableCollection<ProgramPolicyRuleViewModel>();
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

        public void ApplyPendingName()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            policy.Name = _pendingName;
        }

        public void RevertPendingName()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            Name = policy.Name;
        }

        public List<GpuOverclockProfileViewModel> AvailableProfiles => parent?.Profiles?.Where(p => !Profiles.Contains(p))?.ToList() ?? new List<GpuOverclockProfileViewModel>();

        public ObservableCollection<GpuOverclockProfileViewModel> Profiles { get; }
        public ObservableCollection<ProgramPolicyRuleViewModel> Rules { get; }

        // TODO - Attach to HasPendingChanges property event for sub-objects and raise change for this VM's HasPendingChanges
        // TODO - Ability to revert changes to the list of profiles / rules

        public bool HasPendingChanges => Profiles.Any(p => p.HasChanges) || Rules.Any(r => r.HasChanges);

        public void ApplyPendingChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            
        }
    }
}
