namespace maryna_hladka2.Services
{
    public class NetworkService : INetworkService
    {
        private const string Key = "simulate_offline";

        public event EventHandler? StatusChanged;

        public NetworkService()
        {
            Connectivity.Current.ConnectivityChanged += (s, e) => StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        // Перемикач для тестів: імітує відсутність мережі
        public bool SimulateOffline
        {
            get => Preferences.Get(Key, false);
            set
            {
                Preferences.Set(Key, value);
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsOnline =>
            !SimulateOffline && Connectivity.Current.NetworkAccess != NetworkAccess.None;
    }
}