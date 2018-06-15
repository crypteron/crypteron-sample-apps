using System.Data.Entity;
using Crypteron.SampleApps.ConsoleCipherDbEf6CodeFirst.Models.Mapping;

namespace Crypteron.SampleApps.ConsoleCipherDbEf6CodeFirst.Models
{
    /// <summary>
    /// Most of the code here is standard entity framework biolerplate 
    /// code, except for the Crypteron.CipherDb.Session.Create(...) below
    /// in the SecDbContext constructor
    /// </summary>
    public partial class SecDbContext : DbContext
    {
        static SecDbContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SecDbContext>());
        }

        /// <summary>
        /// Create the database context and CipherDB enable it
        /// </summary>
        /// <param name="securityPartition">
        /// The security partition for this particular database context instance. 
        /// This comes into play only if you're using advanced features like multiple security partitions.
        /// For simpler/out-of-the-box cases, you can ignore/eliminate this.</param>
        /// <param name="asRole">
        /// The role on whose behalf this database context will be opened.
        /// This comes into play only if you're using advanced features like role based access controls
        /// within a given security partition.
        /// For simpler/out-of-the-box cases, you can ignore/eliminate this.
        /// </param>
        public SecDbContext(string securityPartition = null, string asRole = null)
            : base("Name=DatabaseEntities")
        {
            // Crypteron CipherDB power-up this session
            Crypteron.CipherDb.Session.Create(this, securityPartition, asRole);
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
