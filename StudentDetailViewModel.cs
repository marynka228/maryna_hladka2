using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using maryna_hladka2.Models;

namespace maryna_hladka2.ViewModels
{
    public class StudentDetailViewModel : IQueryAttributable, INotifyPropertyChanged
    {
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

        public StudentDetailViewModel()
        {
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

            var updated = new Student
            {
                FullName = FullName,
                Group = Group,
                AverageScore = AverageScore
            };

            WeakReferenceMessenger.Default.Send(new StudentUpdatedMessage(_originalStudent, updated));

            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
