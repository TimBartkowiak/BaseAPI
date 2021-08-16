using BaseAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseAPI.Database
{
    public class BaseDbContext : DbContext
    {
        
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging().UseLazyLoadingProxies();
        }
        
        public DbSet<UserEntity> Users { get; set; }
    }
}