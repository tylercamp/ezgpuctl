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

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                settings.Profiles.Select(p => new GpuOverclockProfileViewModel(gpus, p))
            );

            Profiles.Add(defaultProfile);

            Policies = new ObservableCollection<GpuOverclockPolicyViewModel>(
                settings.Policies.Select(p => new GpuOverclockPolicyViewModel(this, p))
            );

            Policies.Add(GpuOverclockPolicyViewModel.GetDefault(this, defaultProfile));
        }

        public ObservableCollection<GpuOverclockPolicyViewModel> Policies { get; private set; }
        public ObservableCollection<GpuOverclockProfileViewModel> Profiles { get; private set; }

        public void AddPolicy(GpuOverclockPolicy policy, GpuOverclockPolicyViewModel vm)
        {
            throw new NotImplementedException();
        }

        public void AddProfile(GpuOverclockProfile profile, GpuOverclockProfileViewModel vm)
        {
            throw new NotImplementedException();
        }
    }
}
