using Floatly.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Floatly.Helpers
{
    internal class ThemeHelper
    {
        public static Color GetColor(ThemeAccent accent)
        {
            return accent switch
            {
                ThemeAccent.Indigo => Color.FromRgb(0x63, 0x66, 0xf1),
                ThemeAccent.DeepIndigo => Color.FromRgb(0x4f,0x46,0xe5),
                ThemeAccent.Purple => Color.FromRgb(0xA2,0x73,0xFF),

                _ => Colors.White
            };
        }
        public static ThemeAccent GetCurrentTheme()
        {
            Color current = ApplicationAccentColorManager.SystemAccent;

            if (current == GetColor(ThemeAccent.Indigo))
                return ThemeAccent.Indigo;

            if (current == GetColor(ThemeAccent.DeepIndigo))
                return ThemeAccent.DeepIndigo;

            if (current == GetColor(ThemeAccent.Purple))
                return ThemeAccent.Purple;

            return ThemeAccent.Indigo;
        }
    }
}
