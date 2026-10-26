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
        private readonly INetworkService _network;
        private readonly ISyncService _sync;
        private readonly ISyncQueueStore _queue;
        private readonly ISyncApi _api;

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

        private string _deepLinkStudentId = "1";
        public string DeepLinkStudentId
        {
            get => _deepLinkStudentId;
            set => SetProperty(ref _deepLinkStudentId, value);
        }

        // Перемикач "Імітувати офлайн"
        public bool IsOffline
        {
            get => _network.SimulateOffline;
            set
            {
                if (_network.SimulateOffline != value)
                {
                    _network.SimulateOffline = value;
                    OnPropertyChanged();
                    _ = RefreshSyncInfoAsync();
                }
            }
        }

        private string _syncInfo = string.Empty;
        public string SyncInfo
        {
            get => _syncInfo;
            private set => SetProperty(ref _syncInfo, value);
        }

        public ICommand TestDeepLinkCommand { get; }
        public ICommand SyncNowCommand { get; }

        public SettingsViewModel(
            IThemeService theme,
            IAppStateService appState,
            IDeepLinkService deepLink,
            INetworkService network,
            ISyncService sync,
            ISyncQueueStore queue,
            ISyncApi api)
        {
            _theme = theme;
            _appState = appState;
            _deepLink = deepLink;
            _network = network;
            _sync = sync;
            _queue = queue;
            _api = api;
            _isDarkTheme = theme.IsDark;

            TestDeepLinkCommand = new Command(() =>
                _deepLink.Handle(new Uri($"studentapp://student/{DeepLinkStudentId}")));

            SyncNowCommand = new Command(async () =>
            {
                await _sync.SyncAsync();
                await RefreshSyncInfoAsync();
            });

            // Коли синхронізація завершилась, оновлюємо інформацію на екрані
            _sync.SyncCompleted += (s, e) =>
                MainThread.BeginInvokeOnMainThread(async () => await RefreshSyncInfoAsync());

            Refresh();
        }

        public void Refresh()
        {
            SessionInfo =
                $"Початок сесії (UTC): {_appState.SessionStartedUtc:HH:mm:ss}\n" +
                $"Поточна сторінка: {_appState.CurrentPage}\n" +
                $"Згортань (Stopped): {_appState.BackgroundCount}\n" +
                $"Повернень (Resumed): {_appState.ResumeCount}";

            _ = RefreshSyncInfoAsync();
        }

        public async Task RefreshSyncInfoAsync()
        {
            var pending = await _queue.GetPendingCountAsync();
            SyncInfo =
                $"Мережа: {(_network.IsOnline ? "онлайн" : "офлайн")}\n" +
                $"Змін у черзі: {pending}\n" +
                $"Записів на сервері (заглушка): {_api.ServerCount}";
        }
    }
}