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
    // note: when presenting a list, the list should not correspond 

    public class SettingsViewModel
    {
        private Settings settings;

        public SettingsViewModel(List<PhysicalGPU> gpus, Settings settings)
        {
            this.settings = settings;

            var defaultProfile = GpuOverclockProfileViewModel.GetDefault(gpus);
            var profileVms = settings.Profiles.Select(p => new GpuOverclockProfileViewModel(gpus, p)).ToList();
            profileVms.Add(defaultProfile);

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                profileVms.OrderBy(p => p.Name)
            );

            Policies = new ObservableCollection<GpuOverclockPolicyViewModel>(
                settings.Policies.Select(p => new GpuOverclockPolicyViewModel(this, p))
            );
            Policies.Add(GpuOverclockPolicyViewModel.GetDefault(defaultProfile));
        }

        public ObservableCollection<GpuOverclockPolicyViewModel> Policies { get; private set; }
        public ObservableCollection<GpuOverclockProfileViewModel> Profiles { get; private set; }
    }
}
