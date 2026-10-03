using maryna_hladka2.ViewModels;
namespace maryna_hladka2;

public partial class StudentDetailPage : ContentPage
{
	public StudentDetailPage()
	{
		InitializeComponent();
        BindingContext = new StudentDetailViewModel();
    }
}