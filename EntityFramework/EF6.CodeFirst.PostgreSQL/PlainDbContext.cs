using System.Data.Entity;
using Crypteron.SampleApps.EF6.CodeFirst.PostgreSQL.Models;

namespace Crypteron.SampleApps.EF6.CodeFirst.PostgreSQL
{
    public partial class PlainDbContext : DbContext
    {
        static PlainDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PlainDbContext>());
        }

        public PlainDbContext()
            : base("Name=PostgresConnectionString")
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Primary Key
            modelBuilder.Entity<User>().HasKey(t => t.OrderId);
            modelBuilder.Entity<User>().Map(u =>
            {
                // Properties
                u.Property(t => t.OrderItem);

                u.Property(t => t.CustomerName);

                u.Property(t => t.Secure_SocialSecurityNumber);

                u.Property(t => t.Secure_LegacyPIN);

                // Table & Column Mappings
                u.ToTable("Users");
                u.Property(t => t.OrderId).HasColumnName("OrderId");
                u.Property(t => t.OrderItem).HasColumnName("OrderItem");
                u.Property(t => t.Timestamp).HasColumnName("Timestamp");
                u.Property(t => t.CustomerName).HasColumnName("CustomerName");
                u.Property(t => t.SecureSearch_CreditCardNumber).HasColumnName("SecureSearch_CreditCardNumber");
                u.Property(t => t.Secure_SocialSecurityNumber).HasColumnName("Secure_SocialSecurityNumber");
                u.Property(t => t.Secure_LegacyPIN).HasColumnName("PIN");
            });
        }
    }
}
