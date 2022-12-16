using GPUControl.ViewModels;
using NvAPIWrapper.GPU;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GPUControl.Controls
{
    /// <summary>
    /// Interaction logic for OcProfileEditorView.xaml
    /// </summary>
    public partial class OcProfileEditorWindow: Window
    {
        public OcProfileEditorWindow()
        {
            InitializeComponent();
        }

        public event Func<string, bool>? NewNameSelected;
    }
}
