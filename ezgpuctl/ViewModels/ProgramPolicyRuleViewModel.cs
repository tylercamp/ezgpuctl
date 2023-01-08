using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public partial class ProgramPolicyRuleViewModel : ObservableObject
    {
        public ProgramPolicyRuleViewModel(ProgramPolicyRule rule)
        {
            programName = rule.ProgramName;
            negated = rule.Negated;
            isRegex = rule.IsRegex;
            caseInsensitive = rule.CaseInsensitive;
        }

        [ObservableProperty]
        private string programName;

        [ObservableProperty]
        private bool negated;

        [ObservableProperty]
        private bool isRegex;

        [ObservableProperty]
        private bool caseInsensitive;

        public ProgramPolicyRule AsAdvancedModelObject => new ProgramPolicyRule()
        {
            ProgramName = programName,
            Negated = negated,
            IsRegex = isRegex,
            CaseInsensitive = caseInsensitive
        };

        public ProgramPolicyRule AsBasicModelObject => new ProgramPolicyRule()
        {
            ProgramName = programName,
            Negated = negated
        };
    }
}
