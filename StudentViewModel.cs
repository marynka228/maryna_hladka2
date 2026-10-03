using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using maryna_hladka2.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace maryna_hladka2.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        // Інкапсульований об'єкт Model
        private readonly Student _student = new();

        public string FullName
        {
            get => _student.FullName;
            set
            {
                if (_student.FullName != value)
                {
                    _student.FullName = value;
                    OnPropertyChanged();
                    // Текст ПІБ змінився - треба оновити і "вітання", і CanExecute кнопки
                    OnPropertyChanged(nameof(Greeting));
                    (AddStudentCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Group
        {
            get => _student.Group;
            set
            {
                if (_student.Group != value)
                {
                    _student.Group = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        public double AverageScore
        {
            get => _student.AverageScore;
            set
            {
                if (_student.AverageScore != value)
                {
                    _student.AverageScore = value;
                    OnPropertyChanged();
                    // Від AverageScore залежить колір/індикація - треба сповістити і про неї
                    OnPropertyChanged(nameof(IsHighAverage));
                }
            }
        }

        // Обчислювана властивість - альтернатива IValueConverter
        // (для одностороннього індикатора "AverageScore >= 4.0")
        public bool IsHighAverage => AverageScore >= 4.0;

        // Форматоване вітання для Label з OneWay-прив'язкою
        public string Greeting => $"Студент: {FullName}, група {Group}";

        public ObservableCollection<Student> Students { get; } = new();

        public ICommand AddStudentCommand { get; }

        public ICommand OpenDetailsCommand { get; }

        public StudentViewModel()
        {
            AddStudentCommand = new RelayCommand(AddStudent, CanAddStudent);
            OpenDetailsCommand = new Command<Student>(async (student) => await OpenDetailsAsync(student));

            WeakReferenceMessenger.Default.Register<StudentUpdatedMessage>(this, (recipient, message) =>
            {
                var index = Students.IndexOf(message.OriginalStudent);
                if (index >= 0)
                {
                    Students[index] = message.UpdatedStudent;
                }
            });
        }

        private async Task OpenDetailsAsync(Student? student)
        {
            if (student is null) return;

            var parameters = new Dictionary<string, object>
    {
        { "SelectedStudent", student }
    };

            await Shell.Current.GoToAsync("studentdetail", parameters);
        }

        private void AddStudent()
        {
            Students.Add(new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            });

            // Очищення полів вводу після додавання
            FullName = string.Empty;
            Group = string.Empty;
            AverageScore = 0;
        }

        private bool CanAddStudent() => !string.IsNullOrWhiteSpace(FullName);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
