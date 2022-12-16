using GPUControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public class ProgramPolicyRuleViewModel : ViewModel
    {
        ProgramPolicyRule rule;

        public ProgramPolicyRuleViewModel(ProgramPolicyRule rule)
        {
            this.rule = rule;

            _pendingProgramName = rule.ProgramName;
            _pendingNegated = rule.Negated;
        }

        private string _pendingProgramName;
        public string ProgramName
        {
            get => _pendingProgramName;
            set
            {
                _pendingProgramName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private bool _pendingNegated;
        public bool Negated
        {
            get => _pendingNegated;
            set
            {
                _pendingNegated = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        public bool HasChanges => _pendingProgramName != rule.ProgramName || _pendingNegated != rule.Negated;

        public void ApplyChanges()
        {
            rule.ProgramName = ProgramName;
            rule.Negated = Negated;
        }

        public void RevertChanges()
        {
            Negated = rule.Negated;
            ProgramName = rule.ProgramName;
        }
    }
}
