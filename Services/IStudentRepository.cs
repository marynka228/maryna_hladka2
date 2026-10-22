using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<int> SaveAsync(Student student);
        Task<int> DeleteAsync(Student student);
    }
}