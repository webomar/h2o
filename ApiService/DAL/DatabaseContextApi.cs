using ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiService.DAL
{
    public class DatabaseContextApi : DbContext
    {
        public DatabaseContextApi(DbContextOptions<DatabaseContextApi> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional model configuration can go here
        }
    }
}
