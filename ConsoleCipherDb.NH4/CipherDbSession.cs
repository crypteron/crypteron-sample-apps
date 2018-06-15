using Crypteron.SampleApps.ConsoleCipherDbNh4.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Crypteron.SampleApps.ConsoleCipherDbNh4
{
    // This is the only non-transparent interface into CipherDB and is used for
    // security related tasks and configuration. Beyond this, the rest of your 
    // your application code is unchanged
    public class CipherDbSession
    {
        private static Configuration _configuration;

        /// <summary>
        /// This is a CipherDB powered NHibernate session.
        /// </summary>
        /// <param name="securityPartition">This is the application domain defined security
        /// partition. Security partitions are defined by the security officer inside the 
        /// Crypteron CipherCore Keychain. Often used to isolate tenants in a multi-tenant
        /// application.</param>
        /// <param name="asRole">This is the application domain defined role. This is used
        ///     against the ACLs security domain ACLs</param>
        public static ISession OpenSession(string securityPartition = null, string asRole = null)
        {
            // Open a CipherDB enabled session
            return Crypteron.CipherDb.NH.Session.Create(Configuration, securityPartition, asRole);
        }

        #region [NHibernate's standard boilerplate code - Unrelated to Crypteron]
        public static void DropAndGenerateSchema()
        {
            // This will create SQL statements (DDL) to export the XML
            // schema into the database. Used here for the creation of the
            // schema in database (it can do more than just creation though)
            new SchemaExport(Configuration).Execute(useStdOut: false, execute: true, justDrop: false);
        }


        private static Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Configuration();
                    _configuration.Configure();
                    _configuration.AddAssembly(typeof(NUser).Assembly);
                }
                return _configuration;
            }
        }

        #endregion
    }
}
