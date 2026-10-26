using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public interface ISyncQueueStore
    {
        event EventHandler? QueueChanged;
        Task<List<SyncQueueItem>> GetPendingAsync();
        Task<int> GetPendingCountAsync();
        Task RemoveAsync(SyncQueueItem item);
    }
}