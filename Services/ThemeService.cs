using Microsoft.Extensions.Logging;

namespace maryna_hladka2.Services
{
    public class ThemeService : IThemeService
    {
        private const string KeyTheme = "app_theme";   // "System" | "Light" | "Dark"
        private readonly ILogger<ThemeService> _logger;

        public ThemeService(ILogger<ThemeService> logger) => _logger = logger;

        public bool IsDark
        {
            get
            {
                var app = Application.Current;
                if (app is null) return false;
                var theme = app.UserAppTheme == AppTheme.Unspecified ? app.RequestedTheme : app.UserAppTheme;
                return theme == AppTheme.Dark;
            }
        }

        public void SetDark(bool isDark)
        {
            Preferences.Set(KeyTheme, isDark ? "Dark" : "Light");
            Apply(isDark ? AppTheme.Dark : AppTheme.Light);
        }

        public void ApplyStoredTheme()
        {
            var stored = Preferences.Get(KeyTheme, "System");
            Apply(stored switch
            {
                "Dark" => AppTheme.Dark,
                "Light" => AppTheme.Light,
                _ => AppTheme.Unspecified
            });
        }

        private void Apply(AppTheme theme)
        {
            if (Application.Current is { } app)
                app.UserAppTheme = theme;
            _logger.LogInformation("Тему застосовано: {Theme}", theme);
        }
    }
}