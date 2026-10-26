namespace maryna_hladka2.Services
{
    public interface ISyncService
    {
        event EventHandler? SyncCompleted;
        void Start();
        Task<int> SyncAsync();
    }
}