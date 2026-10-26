using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    public interface ISyncApi
    {
        int ServerCount { get; }
        Task SendAsync(SyncQueueItem item);
    }
}