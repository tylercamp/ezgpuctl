using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GPUControl.ViewModels;
using GPUControl.Model;
using System.Windows.Controls.Ribbon;
using System.Collections.Specialized;

namespace GPUControl.Controls
{
    public class OcPolicyEditorWindowViewModel : ViewModel
    {
        // for XAML designer
        public OcPolicyEditorWindowViewModel()
        {
            Policy = new GpuOverclockPolicyViewModel();
            AvailableProgramNames = new List<string>();
        }

        public OcPolicyEditorWindowViewModel(GpuOverclockPolicyViewModel policyViewModel)
        {
            Policy = policyViewModel;
            Policy.Profiles.CollectionChanged += OnPolicyProfilesChanged;
            Policy.Rules.CollectionChanged += OnPolicyRulesChanged;
            AvailableProgramNames = ProcessMonitor.ProgramNames!;
        }

        private void OnPolicyProfilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanAddProfile));

            if (Policy.Profiles.Count == 0)
            {
                SelectedProfile = null;
            }
        }

        private void OnPolicyRulesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (Policy.Rules.Count == 0)
            {
                SelectedRule = null;
            }
        }

        public void Dispose()
        {
            Policy.Profiles.CollectionChanged -= OnPolicyProfilesChanged;
            Policy.Rules.CollectionChanged -= OnPolicyRulesChanged;
        }

        public GpuOverclockPolicyViewModel Policy { get; }

        public bool CanAddProfile => Policy.AvailableProfiles.Count > 0;

        public void NotifyForProfilesChanged()
        {
            OnPropertyChanged(nameof(HasSelectedEditableProfile));
            OnPropertyChanged(nameof(CanRemoveProfile));
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
            OnPropertyChanged(nameof(CanMoveToTop));
            OnPropertyChanged(nameof(CanMoveToBottom));
        }

        private GpuOverclockProfileViewModel? _selectedProfile = null;
        public GpuOverclockProfileViewModel? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile != value)
                {
                    _selectedProfile = value;
                    OnPropertyChanged();
                    NotifyForProfilesChanged();
                }
            }
        }

        private ProgramPolicyRuleViewModel? _selectedRule = null;
        public ProgramPolicyRuleViewModel? SelectedRule
        {
            get => _selectedRule;
            set
            {
                _selectedRule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedRule));
            }
        }

        public bool HasSelectedEditableProfile => !Policy.IsReadOnly && SelectedProfile != null && !SelectedProfile.IsReadOnly;

        public bool HasSelectedRule => _selectedRule != null;

        public List<string> AvailableProgramNames { get; set; }

        public bool CanRemoveProfile => SelectedProfile != null && !Policy.IsReadOnly;
        public bool CanMoveUp => SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) > 0;
        public bool CanMoveDown => SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) < Policy.Profiles.Count - 1;
        public bool CanMoveToTop => SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) != 0;
        public bool CanMoveToBottom => SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) != Policy.Profiles.Count - 1;

        public bool CanAddRule => !Policy.IsReadOnly;
    }

    /// <summary>
    /// Interaction logic for OcPolicyEditorWindow.xaml
    /// </summary>
    public partial class OcPolicyEditorWindow : Window
    {
        public OcPolicyEditorWindow()
        {
            InitializeComponent();
        }

        public OcPolicyEditorWindowViewModel ViewModel
        {
            get => (DataContext as OcPolicyEditorWindowViewModel)!;
            set => DataContext = value;
        }

        private void AddProgramButton_Click(object sender, RoutedEventArgs e)
        {
            var newProgramRule = new ProgramPolicyRule { Negated = false, ProgramName = "" };
            var newProgramRuleVm = new ProgramPolicyRuleViewModel(newProgramRule);

            ViewModel.Policy.Rules.Add(newProgramRuleVm);
        }

        private void RemoveProgramButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Policy.Rules.Remove(ViewModel.SelectedRule!);
            ViewModel.SelectedRule = null;
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // https://social.msdn.microsoft.com/Forums/en-US/eeaf2aa6-0c20-4226-a95f-5eb6fb9750c8/open-contextmenu-on-left-mouse-click-wpf-c?forum=wpf
            var btn = (sender as Button)!;
            var menu = btn.ContextMenu;
            menu.PlacementTarget = btn;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
        }

        private void ProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var profileVm = ((sender as MenuItem)!.DataContext as GpuOverclockProfileViewModel)!;
            ViewModel.Policy.Profiles.Add(profileVm);
            ViewModel.NotifyForProfilesChanged();
        }

        private void RemoveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Policy.Profiles.Remove(ViewModel.SelectedProfile!);
            ViewModel.SelectedProfile = null;        }

        private void MoveProfileUpButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!;
            var profileIdx = ViewModel.Policy.Profiles.IndexOf(profile);
            
            ViewModel.Policy.Profiles.Move(profileIdx, profileIdx - 1);
            ViewModel.SelectedProfile = profile;
        }

        private void MoveProfileDownButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!;
            var profileIdx = ViewModel.Policy.Profiles.IndexOf(profile);

            ViewModel.Policy.Profiles.Move(profileIdx, profileIdx + 1);
            ViewModel.SelectedProfile = profile;
        }

        private void MoveProfileTopButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!;
            var profileIdx = ViewModel.Policy.Profiles.IndexOf(profile);

            ViewModel.Policy.Profiles.Move(profileIdx, 0);
            ViewModel.SelectedProfile = profile;
        }

        private void MoveProfileBottomButton_Click(object sender, RoutedEventArgs e)
        {
            var profile = ViewModel.SelectedProfile!;
            var profileIdx = ViewModel.Policy.Profiles.IndexOf(profile);
            
            ViewModel.Policy.Profiles.Move(profileIdx, ViewModel.Policy.Profiles.Count - 1);
            ViewModel.SelectedProfile = profile;
        }
    }
}
