using Microsoft.EntityFrameworkCore;
namespace TodoApi.Models
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }
        public DbSet<ProjectConstructionEntity> ProjectConstructionItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectConstructionEntity>(e =>
            {
                e.ToTable("project_construction", "main");
                e.HasKey(m => m.id);
            });
        }
    }
}