using System.Text.Json.Serialization;
using SQLite;

namespace maryna_hladka2.Models
{
    // Таблиця Student: Id (PK), FullName, Group, AverageScore, DocumentNumberEncrypted
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public double AverageScore { get; set; }

        // Відкритий номер документа: живе тільки в пам'яті, у БД і в чергу синхронізації не потрапляє
        [Ignore, JsonIgnore]
        public string DocumentNumber { get; set; } = string.Empty;

        // Зашифроване значення: саме воно зберігається в БД
        public string DocumentNumberEncrypted { get; set; } = string.Empty;
    }
}