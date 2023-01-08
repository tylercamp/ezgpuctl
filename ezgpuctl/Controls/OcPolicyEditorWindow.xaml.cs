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
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Overclock;
using GPUControl.Util;

namespace GPUControl.Controls
{
    public partial class OcPolicyEditorWindowViewModel : ObservableObject
    {
        // for XAML designer
        public OcPolicyEditorWindowViewModel() : this(null)
        {
        }

        public OcPolicyEditorWindowViewModel(GpuOverclockPolicyViewModel? policyViewModel)
        {
            if (policyViewModel != null)
            {
                Policy = policyViewModel ?? new GpuOverclockPolicyViewModel();
                Policy.Profiles.CollectionChanged += OnPolicyProfilesChanged;
                Policy.Rules.CollectionChanged += OnPolicyRulesChanged;
                AvailableProgramNames = ProcessMonitor.LastProgramNames!;
            }
            else
            {
                Policy = new GpuOverclockPolicyViewModel();
                AvailableProgramNames = new List<string>();
            }

            void MoveProfileRelative(int pos)
            {
                var profile = SelectedProfile!;
                var profileIdx = Policy.Profiles.IndexOf(profile);

                Policy.Profiles.Move(profileIdx, profileIdx + pos);
                SelectedProfile = profile;
            }

            void MoveProfileAbsolute(int pos)
            {
                var profile = SelectedProfile!;
                var profileIdx = Policy.Profiles.IndexOf(profile);

                Policy.Profiles.Move(profileIdx, pos);
                SelectedProfile = profile;
            }

            MoveProfileUp = new RelayCommand(
                () => MoveProfileRelative(-1),
                () => !Policy.IsReadOnly && SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) > 0
            );

            MoveProfileDown = new RelayCommand(
                () => MoveProfileRelative(1),
                () => !Policy.IsReadOnly && SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) < Policy.Profiles.Count - 1
            );

            MoveProfileTop = new RelayCommand(
                () => MoveProfileAbsolute(0),
                () => !Policy.IsReadOnly && SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) != 0
            );

            MoveProfileBottom = new RelayCommand(
                () => MoveProfileAbsolute(Policy.Profiles.Count - 1),
                () => !Policy.IsReadOnly && SelectedProfile != null && Policy.Profiles.IndexOf(SelectedProfile) != Policy.Profiles.Count - 1
            );

            RemoveProfile = new RelayCommand(
                () => { Policy.Profiles.Remove(SelectedProfile!); SelectedProfile = null; },
                () => SelectedProfile != null
            );

            RemoveRule = new RelayCommand(
                () => { Policy.Rules.Remove(SelectedRule!); SelectedRule = null; },
                () => SelectedRule != null
            );
        }

        public IRelayCommand MoveProfileUp { get; }
        public IRelayCommand MoveProfileDown { get; }
        public IRelayCommand MoveProfileTop { get; }
        public IRelayCommand MoveProfileBottom { get; }
        public IRelayCommand RemoveProfile { get; }
        public IRelayCommand RemoveRule { get; }
        

        private void OnPolicyProfilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            MoveProfileUp.NotifyCanExecuteChanged();
            MoveProfileDown.NotifyCanExecuteChanged();
            MoveProfileTop.NotifyCanExecuteChanged();
            MoveProfileBottom.NotifyCanExecuteChanged();

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

        public bool CanAddProfile => Policy.AvailableProfiles.Count > 0;

        public GpuOverclockPolicyViewModel Policy { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MoveProfileUp))]
        [NotifyCanExecuteChangedFor(nameof(MoveProfileDown))]
        [NotifyCanExecuteChangedFor(nameof(MoveProfileTop))]
        [NotifyCanExecuteChangedFor(nameof(MoveProfileBottom))]
        [NotifyCanExecuteChangedFor(nameof(RemoveProfile))]
        private GpuOverclockProfileViewModel? selectedProfile = null;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveRule))]
        private ProgramPolicyRuleViewModel? selectedRule = null;

        public List<string> AvailableProgramNames { get; set; }
    }

    /// <summary>
    /// Interaction logic for OcPolicyEditorWindow.xaml
    /// </summary>
    public partial class OcPolicyEditorWindow : Window
    {
        public OcPolicyEditorWindow()
        {
            InitializeComponent();
            OriginalName = "";
        }

        public OcPolicyEditorWindow(string originalPolicyName)
        {
            OriginalName = originalPolicyName;
            InitializeComponent();
        }

        public OcPolicyEditorWindowViewModel ViewModel
        {
            get => (DataContext as OcPolicyEditorWindowViewModel)!;
            set => DataContext = value;
        }

        public string OriginalName { get; }

        public event Func<string, bool>? NewNameSelected;

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.Dispose();
        }

        private void AddProgramButton_Click(object sender, RoutedEventArgs e)
        {
            var newProgramRule = new ProgramPolicyRule { Negated = false, ProgramName = "" };
            var newProgramRuleVm = new ProgramPolicyRuleViewModel(newProgramRule);

            ViewModel.Policy.Rules.Add(newProgramRuleVm);
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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Policy.Name != OriginalName && NewNameSelected?.Invoke(ViewModel.Policy.Name) != true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"The name \"{ViewModel.Policy.Name}\" is already in use by another policy.");
                return;
            }

            var validationErrors = ViewModel.Policy.GetValidationErrors();
            if (validationErrors.Count > 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"Errors were found while validating program rules:\n\n" + string.Join(", ", validationErrors));
                return;
            }

            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
