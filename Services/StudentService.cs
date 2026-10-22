using System.Collections.ObjectModel;
using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public class StudentService : IStudentService
    {
        private Task? _loadTask;

        public ObservableCollection<Student> Students { get; } = new();

        // Завантаження виконується лише один раз
        public Task EnsureLoadedAsync() => _loadTask ??= LoadCoreAsync();

        private async Task LoadCoreAsync()
        {
            await Task.Delay(300); // імітація звернення до БД або API

            Students.Add(new Student { Id = 1, FullName = "Іваненко Іван Іванович", Group = "КН-31", AverageScore = 4.5 });
            Students.Add(new Student { Id = 2, FullName = "Петренко Олена Сергіївна", Group = "КН-32", AverageScore = 3.8 });
            Students.Add(new Student { Id = 3, FullName = "Коваль Андрій Петрович", Group = "КН-31", AverageScore = 4.9 });
        }

        public Student? FindById(int id) => Students.FirstOrDefault(s => s.Id == id);

        public int NextId() => Students.Count == 0 ? 1 : Students.Max(s => s.Id) + 1;
    }
}