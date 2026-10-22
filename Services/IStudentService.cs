using System.Collections.ObjectModel;
using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public interface IStudentService
    {
        ObservableCollection<Student> Students { get; }
        Task EnsureLoadedAsync();
        Student? FindById(int id);
        int NextId();
    }
}