using System.ComponentModel.DataAnnotations;

namespace ToDo.DAL.SQLiteEntities
{
    public class LocalTask
    {
        [Key]
        public int LocalId { get; set; }

        public int? SqlServerId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public string Priority { get; set; } = null!;

        public string Status { get; set; } = null!;

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SyncStatus { get; set; } = "Pending";

        public DateTime? SyncedAt { get; set; }
    }
}
