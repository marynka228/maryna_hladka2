using maryna_hladka2.ViewModels;
using maryna_hladka2.Views;
using Microsoft.Extensions.Logging;
using maryna_hladka2.Services;

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
            builder.Services.AddSingleton<IStudentRepository, StudentRepository>();

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
