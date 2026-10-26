using System.Text.Json;
using SQLite;
using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public class StudentRepository : IStudentRepository, ISyncQueueStore
    {
        private readonly string _dbPath = Path.Combine(FileSystem.AppDataDirectory, "students.db3");
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private readonly IEncryptionService _crypto;
        private SQLiteAsyncConnection? _db;

        public event EventHandler? QueueChanged;

        public StudentRepository(IEncryptionService crypto) => _crypto = crypto;

        private async Task<SQLiteAsyncConnection> GetDbAsync()
        {
            if (_db is not null) return _db;

            await _initLock.WaitAsync();
            try
            {
                if (_db is not null) return _db;

                var db = new SQLiteAsyncConnection(_dbPath);
                await db.CreateTableAsync<Student>();
                await db.CreateTableAsync<SyncQueueItem>();

                if (await db.Table<Student>().CountAsync() == 0)
                {
                    var seed = new[]
                    {
                        new Student { FullName = "Іваненко Іван Іванович", Group = "КН-31", AverageScore = 4.5, DocumentNumber = "AB123456" },
                        new Student { FullName = "Петренко Олена Сергіївна", Group = "КН-32", AverageScore = 3.8, DocumentNumber = "CD654321" },
                        new Student { FullName = "Коваль Андрій Петрович", Group = "КН-31", AverageScore = 4.9, DocumentNumber = "EF111222" }
                    };
                    foreach (var s in seed)
                        s.DocumentNumberEncrypted = await _crypto.EncryptAsync(s.DocumentNumber);

                    await db.InsertAllAsync(seed);
                }

                _db = db;
                return db;
            }
            finally
            {
                _initLock.Release();
            }
        }

        // ----- CRUD студентів -----

        public async Task<List<Student>> GetAllAsync()
        {
            var db = await GetDbAsync();
            var list = await db.Table<Student>().ToListAsync();
            foreach (var s in list)
                s.DocumentNumber = await _crypto.DecryptAsync(s.DocumentNumberEncrypted);
            return list;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            var db = await GetDbAsync();
            var student = await db.FindAsync<Student>(id);
            if (student is not null)
                student.DocumentNumber = await _crypto.DecryptAsync(student.DocumentNumberEncrypted);
            return student;
        }

        public async Task<int> SaveAsync(Student student)
        {
            var db = await GetDbAsync();
            var isNew = student.Id == 0;

            // Шифруємо перед записом у БД
            student.DocumentNumberEncrypted = await _crypto.EncryptAsync(student.DocumentNumber);

            var result = isNew
                ? await db.InsertAsync(student)
                : await db.UpdateAsync(student);

            await EnqueueAsync(db, isNew ? "Insert" : "Update", student);
            return result;
        }

        public async Task<int> DeleteAsync(Student student)
        {
            var db = await GetDbAsync();
            var result = await db.DeleteAsync(student);
            await EnqueueAsync(db, "Delete", student);
            return result;
        }

        // У чергу йде JSON без відкритого номера документа (він має [JsonIgnore]), лише зашифрований
        private async Task EnqueueAsync(SQLiteAsyncConnection db, string operation, Student student)
        {
            await db.InsertAsync(new SyncQueueItem
            {
                EntityType = nameof(Student),
                Operation = operation,
                PayloadJson = JsonSerializer.Serialize(student),
                CreatedAt = DateTime.UtcNow
            });
            QueueChanged?.Invoke(this, EventArgs.Empty);
        }

        // ----- Черга синхронізації -----

        public async Task<List<SyncQueueItem>> GetPendingAsync()
        {
            var db = await GetDbAsync();
            return await db.Table<SyncQueueItem>().OrderBy(i => i.Id).ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            var db = await GetDbAsync();
            return await db.Table<SyncQueueItem>().CountAsync();
        }

        public async Task RemoveAsync(SyncQueueItem item)
        {
            var db = await GetDbAsync();
            await db.DeleteAsync(item);
        }
    }
}