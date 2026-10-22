using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using maryna_hladka2.Models;
using maryna_hladka2.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace maryna_hladka2.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly Student _student = new();
        private readonly IAppStateService _appState;
        private readonly IStudentService _studentService;

        public string FullName
        {
            get => _student.FullName;
            set
            {
                if (_student.FullName != value)
                {
                    _student.FullName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Greeting));
                    (AddStudentCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    SyncDraft();
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
                    SyncDraft();
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
                    OnPropertyChanged(nameof(IsHighAverage));
                    SyncDraft();
                }
            }
        }

        public bool IsHighAverage => AverageScore >= 4.0;

        public string Greeting => $"Студент: {FullName}, група {Group}";

        // Список тепер живе в спільному сервісі
        public ObservableCollection<Student> Students => _studentService.Students;

        public ICommand AddStudentCommand { get; }
        public ICommand OpenDetailsCommand { get; }
        public ICommand LoadCommand { get; }

        public StudentViewModel(IAppStateService appState, IStudentService studentService)
        {
            _appState = appState;
            _studentService = studentService;

            _student.FullName = _appState.Draft.FullName;
            _student.Group = _appState.Draft.Group;
            _student.AverageScore = _appState.Draft.AverageScore;

            AddStudentCommand = new RelayCommand(AddStudent, CanAddStudent);
            OpenDetailsCommand = new Command<Student>(async (student) => await OpenDetailsAsync(student));
            LoadCommand = new Command(async () => await _studentService.EnsureLoadedAsync());

            WeakReferenceMessenger.Default.Register<StudentUpdatedMessage>(this, (recipient, message) =>
            {
                var index = Students.IndexOf(message.OriginalStudent);
                if (index >= 0)
                {
                    Students[index] = message.UpdatedStudent;
                }
            });
        }

        private void SyncDraft()
        {
            _appState.Draft.FullName = _student.FullName;
            _appState.Draft.Group = _student.Group;
            _appState.Draft.AverageScore = _student.AverageScore;
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
                Id = _studentService.NextId(),
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            });

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