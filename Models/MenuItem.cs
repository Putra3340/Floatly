using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui.Controls;

namespace Floatly.Models
{
    public partial class MenuItemModel : ObservableObject
    {
        public string Title { get; set; } = string.Empty;

        public IRelayCommand Command { get; set; } = null!;
    }
}
