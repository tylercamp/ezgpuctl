using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GPUControl.ViewModels
{
    public partial class OcSelectableViewModel : ObservableObject
    {
        public bool IsReadOnly { get; protected set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayFontWeight))]
        private bool isActive;

        public FontWeight DisplayFontWeight => IsActive ? FontWeights.Bold : FontWeights.Normal;

        public FontStyle DisplayFontStyle => IsReadOnly ? FontStyles.Italic : FontStyles.Normal;
    }
}
