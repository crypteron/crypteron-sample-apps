using Crypteron;
using Microsoft.EntityFrameworkCore;

namespace ConsoleCipherObject.EFCore
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.DbConnectionString);
            //optionsBuilder.UseSqlite("Data Source=securedatabase.db");
        }
    }

    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        // support searchable encryption on this field
        [Secure(Opt.Search)]
        public string CreditCardNumber { get; set; }

        [Secure]
        public string SocialSecurityNumber { get; set; }
    }
}
