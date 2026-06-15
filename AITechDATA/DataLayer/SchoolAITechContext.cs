using Microsoft.EntityFrameworkCore;

namespace AITechDATA.DataLayer
{
    public class SchoolAITechContext : AITechContext
    {
        public SchoolAITechContext(DbContextOptions<SchoolAITechContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            base.OnModelCreating(modelBuilder);
        }
    }
}
