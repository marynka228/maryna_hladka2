namespace maryna_hladka2.Services
{
    public class StudentDraft
    {
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }

    public interface IAppStateService
    {
        DateTime SessionStartedUtc { get; }
        string CurrentPage { get; set; }
        int BackgroundCount { get; set; }
        int ResumeCount { get; set; }
        StudentDraft Draft { get; }

        void SaveToPreferences();
        void LoadFromPreferences();
    }
}