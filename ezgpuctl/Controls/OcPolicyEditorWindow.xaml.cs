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

namespace GPUControl.Controls
{
    /// <summary>
    /// Interaction logic for OcPolicyEditorWindow.xaml
    /// </summary>
    public partial class OcPolicyEditorWindow : Window
    {
        public OcPolicyEditorWindow()
        {
            InitializeComponent();
        }

        public GpuOverclockPolicyViewModel ViewModel
        {
            get => (DataContext as GpuOverclockPolicyViewModel)!;
            set => DataContext = value;
        }

        private void AddProgramButton_Click(object sender, RoutedEventArgs e)
        {
            var newProgramRule = new ProgramPolicyRule { Negated = false, ProgramName = "" };
            var newProgramRuleVm = new ProgramPolicyRuleViewModel(newProgramRule);

            ViewModel.Rules.Add(newProgramRuleVm);
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
            ViewModel.Profiles.Add(profileVm);
        }
    }
}
