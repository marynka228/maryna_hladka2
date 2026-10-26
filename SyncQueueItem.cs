using SQLite;

namespace maryna_hladka2.Models
{
    // Таблиця SyncQueueItem: Id, EntityType, Operation, PayloadJson, CreatedAt
    public class SyncQueueItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;   // Insert, Update, Delete
        public string PayloadJson { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}