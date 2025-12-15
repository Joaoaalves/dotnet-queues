using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Joaoaalves.Queues.EF.Entities
{
    [Table("QueueJobs")]
    public class QueueJob
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string PayloadJson { get; set; } = "{}";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}