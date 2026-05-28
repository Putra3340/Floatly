using Floatly.Enums;
using Floatly.Helpers;
using System.Windows.Media;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Floatly.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;
        [ObservableProperty]
        private ThemeAccent _currentAccent = ThemeAccent.Indigo;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            CurrentAccent = ThemeHelper.GetCurrentTheme();
            AppVersion = $"Floatly - {GetAssemblyVersion()}";
            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }
        [RelayCommand]
        private void OnChangeAccent(string parameter)
        {
            switch (parameter)
            {
                case "deep_indigo":
                    CurrentAccent = ThemeAccent.DeepIndigo;
                    break;

                case "purple":
                    CurrentAccent = ThemeAccent.Purple;
                    break;

                default:
                    CurrentAccent = ThemeAccent.Indigo;
                    break;
            }

            ApplicationAccentColorManager.Apply(
    ThemeHelper.GetColor(CurrentAccent),
    ApplicationThemeManager.GetAppTheme()
);

            ApplicationThemeManager.Apply(
                ApplicationThemeManager.GetAppTheme(),
                WindowBackdropType.Mica,
                true
            );
        }
    }
}
