using maryna_hladka2.Services;
using maryna_hladka2.ViewModels;

namespace maryna_hladka2.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _vm;
        private readonly IAppStateService _appState;

        public SettingsPage(SettingsViewModel vm, IAppStateService appState)
        {
            InitializeComponent();
            _vm = vm;
            _appState = appState;
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _appState.CurrentPage = nameof(SettingsPage);
            _vm.Refresh();
        }
    }
}