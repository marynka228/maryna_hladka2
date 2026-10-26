namespace maryna_hladka2.Services
{
    public interface INetworkService
    {
        bool IsOnline { get; }
        bool SimulateOffline { get; set; }
        event EventHandler? StatusChanged;
    }
}