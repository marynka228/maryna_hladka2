using Microsoft.Extensions.Logging;

namespace maryna_hladka2.Services
{
    public class AppStateService : IAppStateService
    {
        private const string KeyName = "draft_fullname";
        private const string KeyGroup = "draft_group";
        private const string KeyScore = "draft_score";
        private const string KeyLastPage = "last_page";

        private readonly ILogger<AppStateService> _logger;

        public DateTime SessionStartedUtc { get; } = DateTime.UtcNow;
        public string CurrentPage { get; set; } = string.Empty;
        public int BackgroundCount { get; set; }
        public int ResumeCount { get; set; }
        public StudentDraft Draft { get; } = new();

        public AppStateService(ILogger<AppStateService> logger)
        {
            _logger = logger;
            LoadFromPreferences();
        }

        public void SaveToPreferences()
        {
            Preferences.Set(KeyName, Draft.FullName ?? string.Empty);
            Preferences.Set(KeyGroup, Draft.Group ?? string.Empty);
            Preferences.Set(KeyScore, Draft.AverageScore);
            Preferences.Set(KeyLastPage, CurrentPage ?? string.Empty);
            _logger.LogInformation("Стан збережено: '{Name}', '{Group}', {Score}",
                Draft.FullName, Draft.Group, Draft.AverageScore);
        }

        public void LoadFromPreferences()
        {
            Draft.FullName = Preferences.Get(KeyName, string.Empty);
            Draft.Group = Preferences.Get(KeyGroup, string.Empty);
            Draft.AverageScore = Preferences.Get(KeyScore, 0.0);
            CurrentPage = Preferences.Get(KeyLastPage, string.Empty);
            _logger.LogInformation("Стан відновлено: '{Name}', '{Group}', {Score}",
                Draft.FullName, Draft.Group, Draft.AverageScore);
        }
    }
}