using Crypteron.SampleApps.ConsoleCipherDbNh4.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Crypteron.SampleApps.ConsoleCipherDbNh4
{
    // This is the only non-transparent interface into CipherDB and is used for
    // security related tasks and configuration. Beyond this, the rest of your 
    // your application code is unchanged
    public class PlainNHibernateSession
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration _configuration;

        #region [NHibernate's general boilerplate code]

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

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

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    // very expensive, run just once!
                    // Even on a local SQLCE it can take 400ms (cold run) 
                    // to 40 ms (warm run i.e. blind rerun method)
                    _sessionFactory = Configuration.BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        #endregion
    }
}
