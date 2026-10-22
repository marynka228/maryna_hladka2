using Microsoft.Extensions.Logging;

namespace maryna_hladka2.Services
{
    // Підтримуване посилання: studentapp://student/{id}
    public class DeepLinkService : IDeepLinkService
    {
        private readonly IStudentService _students;
        private readonly ILogger<DeepLinkService> _logger;
        private Uri? _pending;

        public DeepLinkService(IStudentService students, ILogger<DeepLinkService> logger)
        {
            _students = students;
            _logger = logger;
        }

        public void Handle(Uri uri)
        {
            _logger.LogInformation("Отримано deep link: {Uri}", uri);
            _pending = uri; // на холодному старті Shell ще немає, тому відкладаємо
            MainThread.BeginInvokeOnMainThread(async () => await TryProcessPendingAsync());
        }

        public async Task TryProcessPendingAsync()
        {
            var shell = Application.Current?.Windows.FirstOrDefault()?.Page as Shell;
            if (_pending is null || shell is null) return;

            var uri = _pending;
            _pending = null;

            if (uri.Scheme != "studentapp" || uri.Host != "student")
            {
                _logger.LogWarning("Невідомий deep link: {Uri}", uri);
                return;
            }

            var last = uri.Segments.LastOrDefault()?.Trim('/');
            if (!int.TryParse(last, out var id))
            {
                _logger.LogWarning("У посиланні немає коректного Id: {Uri}", uri);
                return;
            }

            await _students.EnsureLoadedAsync();
            var student = _students.FindById(id);
            if (student is null)
            {
                await shell.DisplayAlert("Deep link", $"Студента з Id={id} не знайдено", "OK");
                return;
            }

            _logger.LogInformation("Deep link: відкриваємо студента {Name}", student.FullName);
            await shell.GoToAsync("studentdetail", new Dictionary<string, object>
            {
                { "SelectedStudent", student }
            });
        }
    }
}