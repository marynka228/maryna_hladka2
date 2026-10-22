using Microsoft.Extensions.Logging;
using maryna_hladka2.Services;
using maryna_hladka2.ViewModels;

namespace maryna_hladka2.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly StudentViewModel _vm;
        private readonly IAppStateService _appState;
        private readonly IDeepLinkService _deepLink;
        private readonly ILogger<MainPage> _logger;

        public MainPage(StudentViewModel vm, IAppStateService appState, IDeepLinkService deepLink, ILogger<MainPage> logger)
        {
            InitializeComponent();
            _vm = vm;
            _appState = appState;
            _deepLink = deepLink;
            _logger = logger;
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _logger.LogInformation("MainPage.OnAppearing: сторінка стала видимою");
            _appState.CurrentPage = nameof(MainPage);

            _vm.LoadCommand.Execute(null);
            await _deepLink.TryProcessPendingAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _logger.LogInformation("MainPage.OnDisappearing: сторінка прихована");
        }
    }
}