using System.Text.Json;
using Microsoft.Extensions.Logging;
using maryna_hladka2.Models;

namespace maryna_hladka2.Services
{
    // Заглушка сервера. Зберігає записи в пам'яті.
    // Стратегія конфліктів: last write wins, тобто пізніша зміна перезаписує попередню.
    public class FakeSyncApi : ISyncApi
    {
        private readonly Dictionary<int, string> _server = new();
        private readonly ILogger<FakeSyncApi> _logger;

        public FakeSyncApi(ILogger<FakeSyncApi> logger) => _logger = logger;

        public int ServerCount => _server.Count;

        public async Task SendAsync(SyncQueueItem item)
        {
            await Task.Delay(200);   // імітація мережевого запиту

            var student = JsonSerializer.Deserialize<Student>(item.PayloadJson)!;

            if (item.Operation == "Delete")
                _server.Remove(student.Id);
            else
                _server[student.Id] = item.PayloadJson;   // Insert/Update: останній запис перемагає

            _logger.LogInformation("Сервер отримав: {Op} студента Id={Id}", item.Operation, student.Id);
        }
    }
}