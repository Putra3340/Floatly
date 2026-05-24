using Floatly.Enums;
using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace Floatly.Helpers
{
    internal class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
            }

            try
            {

                if (Enum.IsDefined(typeof(ApplicationTheme), value))
                {
                    var enumValue = Enum.Parse(typeof(ApplicationTheme), enumString);
                    return enumValue.Equals(value);
                }
            }
            catch { }
            try
            {

                if (Enum.IsDefined(typeof(ThemeAccent), value))
                {
                    var enumValue = Enum.Parse(typeof(ThemeAccent), enumString);
                    return enumValue.Equals(value);
                }
            }
            catch { }
            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
            {
                throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
            }

            return Enum.Parse(typeof(ApplicationTheme), enumString);
        }
    }
}
