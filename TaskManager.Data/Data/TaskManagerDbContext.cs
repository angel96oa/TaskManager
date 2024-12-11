using Microsoft.EntityFrameworkCore;

namespace TaskManager.Data
{
    public class TaskManagerDbContext : DbContext
    {
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
        }
        public DbSet<TaskElement> TaskElements { get; set; }
    }
}
