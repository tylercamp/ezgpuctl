using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Model;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GPUControl.ViewModels
{
    public partial class GpuOverclockPolicyViewModel : OcSelectableViewModel
    {
        SettingsViewModel? parent;

        // for XAML design view only!
        public GpuOverclockPolicyViewModel()
        {
            name = "New Policy";
            parent = null;

            profiles = new ObservableCollection<GpuOverclockProfileViewModel>();
            profiles.Add(new GpuOverclockProfileViewModel());

            rules = new ObservableCollection<ProgramPolicyRuleViewModel>();
            rules.Add(new ProgramPolicyRuleViewModel(new ProgramPolicyRule()
            {
                Negated = true,
                ProgramName = "program.exe"
            }));
        }

        // for normal use
        public GpuOverclockPolicyViewModel(SettingsViewModel? parent, GpuOverclockPolicy policy)
        {
            this.parent = parent;

            name = policy.Name;

            IsReadOnly = false;

            profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                from profileName in policy.OrderedProfileNames
                join profileVm in parent.Profiles
                on profileName equals profileVm.Name
                select profileVm
            );

            rules = new ObservableCollection<ProgramPolicyRuleViewModel>(policy.Rules.Select(r => new ProgramPolicyRuleViewModel(r)));
        }


        // for default policy
        private GpuOverclockPolicyViewModel(GpuOverclockPolicy policy, IEnumerable<GpuOverclockProfileViewModel> profileVms)
        {
            this.parent = null;

            IsReadOnly = true;

            name = policy.Name;

            profiles = new ObservableCollection<GpuOverclockProfileViewModel>(profileVms);
            rules = new ObservableCollection<ProgramPolicyRuleViewModel>();
        }

        public static readonly string DefaultName = "Default Policy";

        public static GpuOverclockPolicyViewModel GetDefault(GpuOverclockProfileViewModel defaultProfile)
        {
            var policy = new Model.GpuOverclockPolicy(DefaultName)
            {
                OrderedProfileNames = new List<string>() { defaultProfile.Name },
                Rules = new List<Model.ProgramPolicyRule>(),
            };

            return new GpuOverclockPolicyViewModel(policy, new List<GpuOverclockProfileViewModel>() { defaultProfile });
        }

        [ObservableProperty]
        private bool isSelectedSpecifically = false;

        [ObservableProperty]
        private string name;

        public List<GpuOverclockProfileViewModel> AvailableProfiles => parent?.Profiles?.Where(p => !p.IsReadOnly && !Profiles.Contains(p))?.ToList() ?? new List<GpuOverclockProfileViewModel>();

        [ObservableProperty]
        private ObservableCollection<GpuOverclockProfileViewModel> profiles;

        [ObservableProperty]
        private ObservableCollection<ProgramPolicyRuleViewModel> rules;

        public GpuOverclockPolicy AsModelObject => new GpuOverclockPolicy(name)
        {
            OrderedProfileNames = Profiles.Select(pvm => pvm.Name).ToList(),
            Rules = Rules.Where(r => r.ProgramName.Trim().Length > 0).Select(rvm => rvm.AsModelObject).ToList()
        };
    }
}
