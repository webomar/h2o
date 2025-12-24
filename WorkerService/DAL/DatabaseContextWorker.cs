using ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace WorkerService.DAL
{
    public class DatabaseContextWorker : DbContext
    {
        public DatabaseContextWorker(DbContextOptions<DatabaseContextWorker> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configuration can go here
        }
    }
}
