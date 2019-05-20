using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crypteron.Internal.Entropy;
using Crypteron.SampleApps.CommonCode;
using Crypteron.SampleApps.ConsoleCipherDbNh4.Domain;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics;

namespace Crypteron.SampleApps.ConsoleCipherDbNh4
{
    public class ProcessCustomer
    {
        public void Create()
        {
            var dbUsr = AddOrEdit();
            using (ISession session = CipherDbSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(dbUsr);
                transaction.Commit();
            }
        }

        public void CreateAuto(int numToAdd)
        {
            using (ISession session = CipherDbSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Console.Write("[CipherDB] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    session.Save(CreateRandomUser());
                    Console.Write(".");

                }
                Console.WriteLine("]");
                transaction.Commit();
            }
        }

        public void CreateAutoInsecure(int numToAdd)
        {
            using (ISession session = PlainNHibernateSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Console.Write("[Non-Secure] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    session.Save(CreateRandomUser());
                    Console.Write(".");

                }
                Console.WriteLine("]");
                transaction.Commit();
            }
        }

        public int ReadAll()
        {
            return ReadAll(true);
        }

        public int ReadAllInsecure()
        {
            return ReadAllInsecure(true);
        }

        public void Update()
        {
            using (ISession session = CipherDbSession.OpenSession())
            {
                var id = GetId();
                var beforeUser = GetById(id);
                if (beforeUser != null)
                {
                    var afterUser = AddOrEdit(beforeUser);
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Update(afterUser);
                        transaction.Commit();
                    }
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void Delete()
        {
            using (ISession session = CipherDbSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var id = GetId();
                var deleteUser = GetById(id);
                if (deleteUser != null)
                {
                    session.Delete(deleteUser);
                    transaction.Commit();
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void DeleteAll()
        {
            using (ISession session = CipherDbSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                int deleted = 0;
                var allUsers = ReadAll<NUser>(session);
                foreach (var user in allUsers)
                {
                    session.Delete(user);
                    deleted++;
                }
                transaction.Commit();
                Console.WriteLine("Deleted {0} entities", deleted);
            }
        }

        public void WipeAllViaSql()
        {
            using (ISession session = CipherDbSession.OpenSession())
            {
                // Create a SQL Command
                System.Data.IDbCommand command = session.Connection.CreateCommand();
                // Set the query you're going to run
                // "TRUNCATE Users" possible (and faster!) too *IF* no 
                // Foreign Key references in design
                command.CommandText = "DELETE FROM NUsers"; 
                // Run the query
                command.ExecuteNonQuery();
            }
        }

        public void LiveMigrate()
        {
            using (ISession session = CipherDbSession.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                foreach (var o in ReadAll<NUser>(session))
                {
                    // If CipherDB detects any rows still using the old
                    // encryption keys AND if Migration Policy allows it
                    // track all such older rows.
                }
                // To minimize write pressure, CipherDB will update old 
                // entities to newer the latest encryption key
                // ONLY when application generates a write, like below

                transaction.Commit();
            }
        }

        public void Benchmark()
        {
            Benchmark(WipeAllViaSql, CreateAuto, ReadAll, CreateAutoInsecure, ReadAllInsecure);
        }

        public void StoredProcedure()
        {
            Console.WriteLine("Enter first few characters of customer name to search:");
            var custName = Console.ReadLine();

            using (ISession session = CipherDbSession.OpenSession())
            {
                var query = session.GetNamedQuery("usp_SearchUserByName");
                query.SetParameter("UsernamePrefix", custName);
                var results = query.List<NUser>();                
                foreach (var r in results)
                    DisplayEntity(r);
            }
        }

        public void EncryptedSearchServerSide()
        {
            // Fast searches done at database side, usually for high frequency search patterns (e.g. search by last name etc)
            // Please read more https://www.crypteron.com/blog/practical-searchable-encryption-and-security/
            // 
            Console.WriteLine("For details, partial searches, fuzzy searches, please read:");
            Console.WriteLine("    https://www.crypteron.com/blog/practical-searchable-encryption-and-security/ \n");
            Console.WriteLine("Credit Card numbers are AES encrypted; we'll do a full speed server side search!");
            Console.Write("Enter the full credit card to search for (including the -'s): ");
            var ccSearchStr = Console.ReadLine();
            using (ISession session = CipherDbSession.OpenSession())
            {
                var creditCardSearchPrefix = SecureSearch.GetPrefix(ccSearchStr);

                foreach (var user in session.Query<NUser>().Where(u => u.SecureSearch_CreditCardNumber.StartsWith(creditCardSearchPrefix)))
                {
                    DisplayEntity(user);
                }
            }
        }

        public void Test()
        {
            // Placeholder for anything you want ...
            throw new NotImplementedException();
        }

        private IEnumerable<T> ReadAll<T>(ISession session) where T : class
        {
            var allEntities = session.QueryOver<T>().List();
            return allEntities;
        }

        private int ReadAll(bool printToScreen)
        {
            using (ISession session = CipherDbSession.OpenSession())
            {
                var allUsers = ReadAll<NUser>(session);
                int totalEntries = 0;
                foreach (var o in allUsers)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o);
                }
                return totalEntries;
            }
        }

        private int ReadAllInsecure(bool printToScreen)
        {
            Console.WriteLine("Without CipherDB, data is secured and is undecipherable by anyone - even SQL server itself.");
            int totalEntries = 0;

            using (var session = PlainNHibernateSession.OpenSession())
            {
                // We're dropping out of the ORM intentionally to demonstrate
                // incorrect usage. This is NOT recommended in production
                var query = session.CreateSQLQuery(@"select {l.*} from NUsers as l");
                query.AddEntity("l", typeof(NUser));
                var users = query.List<NUser>();

                foreach (NUser o in users)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o, true);
                }
            }
            return totalEntries;
        }

        private NUser GetById(int userId)
        {
            using (ISession session = CipherDbSession.OpenSession())
                return session.Get<NUser>(userId);
        }

        private NUser CreateRandomUser()
        {
            var rndUser = new NUser();
            rndUser.OrderId = 0; //db overwrites this
            rndUser.OrderItem = UserRandomizer.GetRandomItem();
            rndUser.CustomerName = UserRandomizer.GetRandomNames();
            rndUser.Timestamp = Randomizer.GetRandomTime();
            rndUser.SecureSearch_CreditCardNumber = UserRandomizer.GetRandomCC();
            rndUser.Secure_LegacyPIN = UserRandomizer.GetRandomPIN();
            rndUser.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(UserRandomizer.GetRandomSSN());
            return rndUser;
        }

        private void DisplayEntity(NUser o, bool rawBits = false)
        {
            string ssnString;

            if (o.Secure_SocialSecurityNumber == null)
            {
                ssnString = "null";
            }
            else
            {
                ssnString = rawBits
                                    ? Convert.ToBase64String(o.Secure_SocialSecurityNumber)
                                    : Encoding.UTF8.GetString(o.Secure_SocialSecurityNumber);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("OrderId:{0}, {1} got {2} at {3} using CC {4},SSN={5}, PIN={6}." + Environment.NewLine,
                o.OrderId,
                o.CustomerName,
                o.OrderItem,
                o.Timestamp,
                o.SecureSearch_CreditCardNumber,
                ssnString,
                o.Secure_LegacyPIN);

            sb.AppendLine("---------------------------------------------------------------");
            Console.Write(sb.ToString());
        }

        private int GetId()
        {
            Console.Write("Select Order ID: ");
            int orderId;
            while (!int.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Unable to parse, try again");
            }
            return orderId;
        }

        private NUser AddOrEdit(NUser editUser = null)
        {
            NUser usr;
            if (editUser == null)
            {
                usr = new NUser
                    {
                        OrderId = 0,
                    };
            }
            else
            {
                usr = editUser;
            }

            Console.Write("Customer Name: [{0}]", usr.CustomerName);
            var temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.CustomerName = temp;

            Console.Write("Item purchased: [{0}]", usr.OrderItem);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.OrderItem = temp;

            Console.Write("Credit Card number: [{0}]", usr.SecureSearch_CreditCardNumber);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.SecureSearch_CreditCardNumber = temp;

            Console.Write("SSN: [{0}]", usr.Secure_SocialSecurityNumber==null ? null : Encoding.UTF8.GetString(usr.Secure_SocialSecurityNumber));
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(temp);

            Console.Write("Legacy PIN: [{0}]", usr.Secure_LegacyPIN);
            temp = Console.ReadLine();
            if (!String.IsNullOrEmpty(temp))
                usr.Secure_LegacyPIN = temp;

            usr.Timestamp = DateTime.Now;

            usr = SanitizeUserProperties(usr);
            return usr;
        }

        private NUser SanitizeUserProperties(NUser input)
        {
            input.CustomerName = LimitString(input.CustomerName, 64);
            input.OrderItem = LimitString(input.OrderItem, 64);
            input.SecureSearch_CreditCardNumber = LimitString(input.SecureSearch_CreditCardNumber, 64);
            input.Secure_LegacyPIN = LimitString(input.Secure_LegacyPIN, 64);

            var temp = Encoding.UTF8.GetString(input.Secure_SocialSecurityNumber);
            input.Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(LimitString(temp, 64));
            
            return input;
        }

        private string LimitString(string inputStr, int lengthLimit)
        {
            if (inputStr != null)
            {
                var limit = Math.Min(inputStr.Length, lengthLimit);
                var output = inputStr.Substring(0, limit);
                return output;
            }

            return String.Empty;
        }

        private void Benchmark(Action wipeDb, Action<int> secureWrite, Func<bool, int> secureRead, Action<int> insecureWrite, Func<bool, int> insecureRead)
        {
            ////////////////////////////////////////////////
            // TEST SETUP
            ////////////////////////////////////////////////
            // How many entites? 
            // Maximum: ORM guidelines suggest breaking down into batches for over 500 entities
            // Minimum: Less than 50 entities can be misleading
            const int benchmarkSize = 500;
            float insecureTimeAvgWrite, secureTimeAvgWrite, insecureTimeAvgRead, secureTimeAvgRead;
            var sw = new Stopwatch();
            // Perform warmup to minimize cold-start latencies (eg. n/w connection setup etc)
            Warmup(wipeDb, secureWrite, insecureWrite, secureRead, insecureRead);
            // Reset Db to known state i.e. empty table
            wipeDb();


            ////////////////////////////////////////////////
            // Regular/non-secure mode without CipherDB
            ////////////////////////////////////////////////
            Console.WriteLine("Regular/Non-secure mode ...");
            Console.WriteLine("There is no CipherDB; meaning no encryption and sensitive data is in the clear.");
            // WRITE
            sw.Restart();
            insecureWrite(benchmarkSize);
            sw.Stop();
            insecureTimeAvgWrite = sw.ElapsedMilliseconds / (float)benchmarkSize;
            // READ
            sw.Restart();
            int numPlainEntries = insecureRead(true);
            sw.Stop();
            insecureTimeAvgRead = sw.ElapsedMilliseconds / (float)numPlainEntries;

            // Reset Db to known state i.e. empty table
            wipeDb();

            ////////////////////////////////////////////////
            // CipherDB
            ////////////////////////////////////////////////
            Console.WriteLine("CipherDB mode ...");
            Console.WriteLine("Encryption, tamper protection, key management etc. active. Data encrypted everything except console (decrypted).");
            // WRITE
            sw.Restart();
            secureWrite(benchmarkSize);
            sw.Stop();
            secureTimeAvgWrite = sw.ElapsedMilliseconds / (float)benchmarkSize;
            // READ
            sw.Restart();
            int numSecureEntries = secureRead(true);
            sw.Stop();
            secureTimeAvgRead = sw.ElapsedMilliseconds / (float)numSecureEntries;


            ////////////////////////////////////////////////
            // Reporting
            ////////////////////////////////////////////////
            Console.WriteLine("====================================================");
            Console.WriteLine("     BENCHMARK RESULTS over {0} database records", benchmarkSize);
            Console.WriteLine("     All times are (avg) milliseconds per entity");
            Console.WriteLine("\t\tReads\tWrites");
            // CipherDB
            Console.WriteLine(" Crypteron ->\t{0:F2}\t{1:F2}", secureTimeAvgRead, secureTimeAvgWrite);
            // Regular/non-secure
            Console.WriteLine("No security->\t{0:F2}\t{1:F2}", insecureTimeAvgRead, insecureTimeAvgWrite);
            // Delta
            Console.WriteLine("Difference\t{0:F2}\t{1:F2}", secureTimeAvgRead - insecureTimeAvgRead, secureTimeAvgWrite - insecureTimeAvgWrite);

            if (secureTimeAvgRead - insecureTimeAvgRead < 1.0)
                // This is the usual case
                Console.WriteLine("Read impact of entire security processing pipeline is less than a single millisecond on average!");
            else
            {
                // Rare situation
                Console.WriteLine("Read impact of entire security processing pipeline is {0:F2} milliseconds",
                    secureTimeAvgRead - insecureTimeAvgRead);

                // If here, something is really wrong
                if (secureTimeAvgRead - insecureTimeAvgRead > 10)
                {
                    Console.WriteLine("This should not happen normally. Please contact " +
                                      "support@crypteron.com to report this so we can " +
                                      "investigate. Thanks.");
                }
            }
        }

        private void Warmup(Action wipeDb, Action<int> secureWrite, Action<int> insecureWrite, Func<bool, int> secureRead, Func<bool, int> insecureRead)
        {
            const int warmUpSize = 2;
            insecureWrite(warmUpSize);
            insecureRead(false);
            wipeDb();

            secureWrite(warmUpSize);
            secureRead(false);
        }
    }
}
