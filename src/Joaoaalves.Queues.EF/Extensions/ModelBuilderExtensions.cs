using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Joaoaalves.Queues.EF.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Apply EF mappings from this assembly into the user's ApplicationDbContext modelBuilder.
        /// Usage inside ApplicationDbContext.OnModelCreating:
        /// modelBuilder.AplyQueueMappings();
        /// </summary>
        public static void AplyQueueMappings(this ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}