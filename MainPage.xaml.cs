using maryna_hladka2.ViewModels;

namespace maryna_hladka2.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new StudentViewModel();
        }
    }
}
