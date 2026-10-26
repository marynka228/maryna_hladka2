using Microsoft.Extensions.Logging;
using maryna_hladka2.Services;

namespace maryna_hladka2
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        private readonly IAppStateService _appState;
        private readonly IThemeService _theme;
        private readonly ISyncService _sync;

        public App(ILogger<App> logger, IAppStateService appState, IThemeService theme, ISyncService sync)
        {
            InitializeComponent();
            _logger = logger;
            _appState = appState;
            _theme = theme;
            _sync = sync;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            _theme.ApplyStoredTheme();
            _sync.Start();   // запускаємо стеження за мережею та чергою

            var window = new Window(new AppShell());

            window.Created += (s, e) => _logger.LogInformation("Window.Created: вікно створено");
            window.Activated += (s, e) => _logger.LogInformation("Window.Activated: застосунок на передньому плані");
            window.Deactivated += (s, e) => _logger.LogInformation("Window.Deactivated: втрачено фокус");

            window.Stopped += (s, e) =>
            {
                _appState.BackgroundCount++;
                _logger.LogInformation("Window.Stopped: застосунок згорнуто");
                _appState.SaveToPreferences();
            };

            window.Resumed += (s, e) =>
            {
                _appState.ResumeCount++;
                _logger.LogInformation("Window.Resumed: застосунок повернувся");
            };

            window.Destroying += (s, e) => _logger.LogInformation("Window.Destroying: звільнення ресурсів");

            return window;
        }
    }
}