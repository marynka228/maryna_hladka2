namespace maryna_hladka2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("studentdetail", typeof(StudentDetailPage));
        }
    }
}
