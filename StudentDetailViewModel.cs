using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using maryna_hladka2.Models;
using maryna_hladka2.Services;

namespace maryna_hladka2.ViewModels
{
    public class StudentDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
        private readonly IStudentRepository _repository;
        private Student? _originalStudent;

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { if (_fullName != value) { _fullName = value; OnPropertyChanged(); } }
        }

        private string _group = string.Empty;
        public string Group
        {
            get => _group;
            set { if (_group != value) { _group = value; OnPropertyChanged(); } }
        }

        private double _averageScore;
        public double AverageScore
        {
            get => _averageScore;
            set { if (_averageScore != value) { _averageScore = value; OnPropertyChanged(); } }
        }

        public ICommand GoBackCommand { get; }
        public ICommand SaveCommand { get; }

        public StudentDetailViewModel(IStudentRepository repository)
        {
            _repository = repository;
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("SelectedStudent", out var value) && value is Student student)
            {
                _originalStudent = student;
                FullName = student.FullName;
                Group = student.Group;
                AverageScore = student.AverageScore;
            }
        }

        private async Task SaveAsync()
        {
            if (_originalStudent is null) return;

            _originalStudent.FullName = FullName;
            _originalStudent.Group = Group;
            _originalStudent.AverageScore = AverageScore;

            await _repository.SaveAsync(_originalStudent);   // Id != 0, тому виконується Update

            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}