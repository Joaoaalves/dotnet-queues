using Joaoaalves.Queues.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joaoaalves.Queues.EF.Mappings
{
    public class QueueJobEntityConfiguration : IEntityTypeConfiguration<QueueJob>
    {
        public void Configure(EntityTypeBuilder<QueueJob> builder)
        {
            builder.ToTable("QueueJobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PayloadJson).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}