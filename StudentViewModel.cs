using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using maryna_hladka2.Models;
using maryna_hladka2.Services;

namespace maryna_hladka2.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private readonly Student _student = new();
        private readonly IAppStateService _appState;
        private readonly IStudentRepository _repository;

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

        public ObservableCollection<Student> Students { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public ICommand AddStudentCommand { get; }
        public ICommand OpenDetailsCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LoadCommand { get; }

        public StudentViewModel(IAppStateService appState, IStudentRepository repository)
        {
            _appState = appState;
            _repository = repository;

            _student.FullName = _appState.Draft.FullName;
            _student.Group = _appState.Draft.Group;
            _student.AverageScore = _appState.Draft.AverageScore;

            AddStudentCommand = new RelayCommand(() => _ = AddStudentAsync(), CanAddStudent);
            OpenDetailsCommand = new Command<Student>(async (student) => await OpenDetailsAsync(student));
            DeleteCommand = new Command<Student>(async (student) => await DeleteAsync(student));
            LoadCommand = new Command(async () => await LoadAsync());
        }

        // Читання з БД: очищаємо колекцію і наповнюємо її заново
        private async Task LoadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var list = await _repository.GetAllAsync();
                Students.Clear();
                foreach (var s in list)
                    Students.Add(s);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddStudentAsync()
        {
            var student = new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            };

            await _repository.SaveAsync(student);   // після Insert поле Id заповнюється автоматично
            Students.Add(student);

            FullName = string.Empty;
            Group = string.Empty;
            AverageScore = 0;
        }

        private async Task DeleteAsync(Student? student)
        {
            if (student is null) return;
            await _repository.DeleteAsync(student);
            Students.Remove(student);
        }

        private async Task OpenDetailsAsync(Student? student)
        {
            if (student is null) return;
            await Shell.Current.GoToAsync("studentdetail", new Dictionary<string, object>
            {
                { "SelectedStudent", student }
            });
        }

        private void SyncDraft()
        {
            _appState.Draft.FullName = _student.FullName;
            _appState.Draft.Group = _student.Group;
            _appState.Draft.AverageScore = _student.AverageScore;
        }

        private bool CanAddStudent() => !string.IsNullOrWhiteSpace(FullName);

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}