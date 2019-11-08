using Microsoft.EntityFrameworkCore;

namespace Crypteron.SampleApps.EFCore3
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.DatabaseConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // NOTE: Crypteron.CrypteronConfig.Config.MyCrypteronAccount.AppSecret
            //       must be configured before this is executed. Depending on your 
            //       application logic, this configuration can be done using 
            //       dependency-injection or a factory method or through 
            //       the dbcontext's constructor
            CipherDb.EFCore3.Entities.Activate(modelBuilder);
        }
    }
}
