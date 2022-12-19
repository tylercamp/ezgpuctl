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
        }

        [ObservableProperty]
        private string programName;

        [ObservableProperty]
        private bool negated;

        public ProgramPolicyRule AsModelObject => new ProgramPolicyRule() { ProgramName = programName, Negated = negated };
    }
}
