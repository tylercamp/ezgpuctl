using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Model;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            advancedMode = policy.Rules.Any(r => r.CaseInsensitive || r.IsRegex);
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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRegexColumnSize))]
        [NotifyPropertyChangedFor(nameof(IsCaseInsensitiveColumnSize))]
        private bool advancedMode;

        public int IsRegexColumnSize => advancedMode ? 50 : 0;
        public int IsCaseInsensitiveColumnSize => advancedMode ? 50 : 0;

        public List<string> GetValidationErrors()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                if (rule.ProgramName.Trim().Length == 0)
                {
                    result.Add($"Rule #{i+1} is empty. Enter a value or remove it.");
                }
                else if (rule.IsRegex && advancedMode)
                {
                    try { Regex.Match("", rule.ProgramName); }
                    catch { result.Add($"The regex for rule #{i+1} is invalid: {rule.ProgramName}"); }
                }
            }
            return result;
        }

        public GpuOverclockPolicy AsModelObject => new GpuOverclockPolicy(name)
        {
            OrderedProfileNames = Profiles.Select(pvm => pvm.Name).ToList(),
            Rules = Rules.Where(r => r.ProgramName.Trim().Length > 0).Select(rvm => advancedMode ? rvm.AsAdvancedModelObject : rvm.AsBasicModelObject).ToList()
        };
    }
}
