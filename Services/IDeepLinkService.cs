namespace maryna_hladka2.Services
{
    public interface IDeepLinkService
    {
        void Handle(Uri uri);
        Task TryProcessPendingAsync();
    }
}