using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using maryna_hladka2.Services;

namespace maryna_hladka2
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "studentapp",
        DataHost = "student")]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HandleIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        private static void HandleIntent(Intent? intent)
        {
            var data = intent?.Data?.ToString();
            if (string.IsNullOrEmpty(data)) return;

            IPlatformApplication.Current?.Services
                .GetService<IDeepLinkService>()?
                .Handle(new Uri(data));
        }
    }
}