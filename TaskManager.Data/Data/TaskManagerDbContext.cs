using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Identity;

namespace TaskManager.Data
{
    public class TaskManagerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
        }

        public DbSet<TaskElement> TaskElements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .ToTable("Users");

            builder.Entity<ApplicationRole>()
                .ToTable("Roles");
        }
    }
}
