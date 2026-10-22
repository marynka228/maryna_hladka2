using SQLite;

namespace maryna_hladka2.Models
{
    // Таблиця Students: Id (PK, автоінкремент), FullName, Group, AverageScore
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}