using SQLite;
using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "students.db3");
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private SQLiteAsyncConnection? _db;

        // Підключення і створення таблиці виконуються при першому зверненні
        private async Task<SQLiteAsyncConnection> GetDbAsync()
        {
            if (_db is not null) return _db;

            await _initLock.WaitAsync();
            try
            {
                if (_db is not null) return _db;

                var db = new SQLiteAsyncConnection(_dbPath);
                await db.CreateTableAsync<Student>();

                // Початкові дані, якщо база порожня
                if (await db.Table<Student>().CountAsync() == 0)
                {
                    await db.InsertAllAsync(new[]
                    {
                        new Student { FullName = "Іваненко Іван Іванович", Group = "КН-31", AverageScore = 4.5 },
                        new Student { FullName = "Петренко Олена Сергіївна", Group = "КН-32", AverageScore = 3.8 },
                        new Student { FullName = "Коваль Андрій Петрович", Group = "КН-31", AverageScore = 4.9 }
                    });
                }

                _db = db;
                return db;
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<List<Student>> GetAllAsync()
        {
            var db = await GetDbAsync();
            return await db.Table<Student>().ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            var db = await GetDbAsync();
            return await db.FindAsync<Student>(id);
        }

        public async Task<int> SaveAsync(Student student)
        {
            var db = await GetDbAsync();
            return student.Id == 0
                ? await db.InsertAsync(student)
                : await db.UpdateAsync(student);
        }

        public async Task<int> DeleteAsync(Student student)
        {
            var db = await GetDbAsync();
            return await db.DeleteAsync(student);
        }
    }
}