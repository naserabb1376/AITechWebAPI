using Microsoft.EntityFrameworkCore;

namespace AITechDATA.DataLayer
{
    public class SchoolAITechContext : AITechContext
    {
        public SchoolAITechContext(DbContextOptions<SchoolAITechContext> options)
            : base(options)
        {
        }
    }
}
