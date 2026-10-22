using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using maryna_hladka2.Services;

namespace maryna_hladka2.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly IThemeService _theme;
        private readonly IAppStateService _appState;
        private readonly IDeepLinkService _deepLink;

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                    _theme.SetDark(value);
            }
        }

        private string _sessionInfo = string.Empty;
        public string SessionInfo
        {
            get => _sessionInfo;
            private set => SetProperty(ref _sessionInfo, value);
        }
        private string _deepLinkStudentId = "2";
        public string DeepLinkStudentId
        {
            get => _deepLinkStudentId;
            set => SetProperty(ref _deepLinkStudentId, value);
        }
        public ICommand TestDeepLinkCommand { get; }

        public SettingsViewModel(IThemeService theme, IAppStateService appState, IDeepLinkService deepLink)
        {
            _theme = theme;
            _appState = appState;
            _deepLink = deepLink;
            _isDarkTheme = theme.IsDark;

            TestDeepLinkCommand = new Command(() =>
                _deepLink.Handle(new Uri($"studentapp://student/{DeepLinkStudentId}")));
            Refresh();
        }

        public void Refresh()
        {
            SessionInfo =
                $"Початок сесії (UTC): {_appState.SessionStartedUtc:HH:mm:ss}\n" +
                $"Поточна сторінка: {_appState.CurrentPage}\n" +
                $"Згортань (Stopped): {_appState.BackgroundCount}\n" +
                $"Повернень (Resumed): {_appState.ResumeCount}";
        }
    }
}