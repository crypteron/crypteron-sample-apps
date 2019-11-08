using System.Data.Entity;
using Crypteron.SampleApps.EF6.CodeFirst.Models.Mapping;

namespace Crypteron.SampleApps.EF6.CodeFirst.Models
{
    public partial class PlainDbContext : DbContext
    {
        static PlainDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PlainDbContext>());
        }

        public PlainDbContext()
            : base("Name=DatabaseEntities")
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
