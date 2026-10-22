namespace maryna_hladka2.Services
{
    public interface IThemeService
    {
        bool IsDark { get; }
        void SetDark(bool isDark);
        void ApplyStoredTheme();
    }
}