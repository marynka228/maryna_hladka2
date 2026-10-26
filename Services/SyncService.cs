using Microsoft.Extensions.Logging;

namespace maryna_hladka2.Services
{
    public class SyncService : ISyncService
    {
        private readonly INetworkService _network;
        private readonly ISyncQueueStore _store;
        private readonly ISyncApi _api;
        private readonly ILogger<SyncService> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private bool _started;

        public event EventHandler? SyncCompleted;

        public SyncService(INetworkService network, ISyncQueueStore store, ISyncApi api, ILogger<SyncService> logger)
        {
            _network = network;
            _store = store;
            _api = api;
            _logger = logger;
        }

        // Підписка: синхронізуємо, коли з'явилась мережа або коли додалась нова зміна
        public void Start()
        {
            if (_started) return;
            _started = true;

            _network.StatusChanged += (s, e) => Trigger();
            _store.QueueChanged += (s, e) => Trigger();
            Trigger();
        }

        private void Trigger()
        {
            if (_network.IsOnline)
                _ = SyncAsync();
        }

        public async Task<int> SyncAsync()
        {
            if (!_network.IsOnline)
            {
                _logger.LogInformation("Офлайн: зміни лишаються в черзі");
                return 0;
            }

            var sent = 0;
            await _lock.WaitAsync();
            try
            {
                var pending = await _store.GetPendingAsync();   // у порядку створення
                foreach (var item in pending)
                {
                    if (!_network.IsOnline) break;
                    try
                    {
                        await _api.SendAsync(item);
                        await _store.RemoveAsync(item);          // видаляємо лише успішно відправлене
                        sent++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Не вдалося відправити зміну {Id}, спробуємо пізніше", item.Id);
                        break;
                    }
                }
            }
            finally
            {
                _lock.Release();
            }

            _logger.LogInformation("Синхронізація завершена, відправлено: {Count}", sent);
            SyncCompleted?.Invoke(this, EventArgs.Empty);
            return sent;
        }
    }
}