namespace maryna_hladka2.Models
{
    // Model — "чистий" клас, жодних залежностей від Microsoft.Maui чи System.Windows.Input
    public class Student
    {
        public string FullName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}

