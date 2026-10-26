using Microsoft.Extensions.Logging;
using maryna_hladka2.Services;
using maryna_hladka2.ViewModels;
using maryna_hladka2.Views;

namespace maryna_hladka2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IAppStateService, AppStateService>();
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<IDeepLinkService, DeepLinkService>();

            // Один екземпляр репозиторію, доступний під двома інтерфейсами
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
            builder.Services.AddSingleton<StudentRepository>();
            builder.Services.AddSingleton<IStudentRepository>(sp => sp.GetRequiredService<StudentRepository>());
            builder.Services.AddSingleton<ISyncQueueStore>(sp => sp.GetRequiredService<StudentRepository>());

            builder.Services.AddSingleton<INetworkService, NetworkService>();
            builder.Services.AddSingleton<ISyncApi, FakeSyncApi>();
            builder.Services.AddSingleton<ISyncService, SyncService>();

            builder.Services.AddTransient<StudentViewModel>();
            builder.Services.AddTransient<StudentDetailViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<StudentDetailPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}