using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Crypteron.Internal.Entropy;

// This way to minimize code duplication
#if EF6CODEFIRST
using Crypteron.SampleApps.EF6.CodeFirst.Models;
#endif

#if EF6DBFIRST
using Crypteron.SampleApps.EF6.DbFirst;
#endif

namespace Crypteron.SampleApps.CommonCode
{
    public class ProcessCustomer
    {
        public void Create()
        {
            var dbUsr = CreateOrEditUser();
            using (var secDb = new SecDbContext())
            {
                secDb.Users.Add(dbUsr);
                secDb.SaveChanges();
            }
        }

        public void CreateAuto(int numToAdd)
        {
            using (var secDb = new SecDbContext())
            {
                Console.Write("[CipherDB] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    secDb.Users.Add(CreateRandomUser());
                    Console.Write(".");
                }
                Console.WriteLine("]");
                secDb.SaveChanges();
            }
        }

        public void CreateAutoInsecure(int numToAdd)
        {
            using (var plainDbContext = new PlainDbContext())
            {
                Console.Write("[Non-Secure] Adding user records " + Environment.NewLine + "[");
                for (int i = 0; i < numToAdd; i++)
                {
                    plainDbContext.Users.Add(CreateRandomUser());
                    Console.Write(".");
                }
                Console.WriteLine("]");
                plainDbContext.SaveChanges();
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
            using (var secDb = new SecDbContext())
            {
                var id = GetId();
                var beforeUser = secDb.Users.Find(id);
                if (beforeUser != null)
                {
                    var afterUser = CreateOrEditUser(beforeUser);
                    secDb.Entry(beforeUser).CurrentValues.SetValues(afterUser);
                    secDb.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void Delete()
        {
            using (var secDb = new SecDbContext())
            {
                var id = GetId();
                var deleteThis = secDb.Users.Find(id);
                if (deleteThis != null)
                {
                    secDb.Users.Remove(deleteThis);
                    secDb.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Order ID {0} not found!", id);
                }
            }
        }

        public void DeleteAll()
        {
            using (var secDb = new SecDbContext())
            {
                int deleted = 0;
                foreach (var deleteThis in secDb.Users)
                {
                    secDb.Users.Remove(deleteThis);
                    deleted++;
                }
                
                var written = secDb.SaveChanges();
                Console.WriteLine("Deleted {0} entities, write to {1} entites", deleted, written);
            }
        }

        public void WipeAllViaSql()
        {
            // Deletions can be done directly without passing 
            // through CipherDB 
            const string rawSqlCmd = "DELETE FROM Users";
            using (var secDb = new SecDbContext())
            {
                secDb.Database.ExecuteSqlCommand(rawSqlCmd);
            }

            Console.WriteLine("Wiped entire table via SQL command: {0}", rawSqlCmd);
        }
        
        public void LiveMigrate()
        {
            using (var secDb = new SecDbContext())
            {
                foreach (User o in secDb.Users)
                {
                    // If CipherDB detects any rows still using the old
                    // encryption keys AND if Migration Policy allows it
                    // track all such older rows.
                }
                // To minimize write pressure, CipherDB will update old 
                // entities to newer the latest encryption key
                // ONLY when application generates a write, like below
                secDb.SaveChanges();
            }
        }

        public void Benchmark()
        {
            Benchmark(WipeAllViaSql, CreateAuto, ReadAll, CreateAutoInsecure, ReadAllInsecure);
        }

        public void StoredProcedure()
        {
#if ConsoleDbFirst
            Console.WriteLine("Enter first few characters of customer name to search:");
            var custName = Console.ReadLine();

            using (var secDbCtx = new SecDbContext())
            {
                // 1. The SQL Stored Proc needs to be imported into EntityFramework
                // 2. The resulting object from the SP needs to be mapped to an 
                //    EntityFramework entity
                var results = secDbCtx.usp_SearchUserByName(custName);
                foreach (var r in results)
                    DisplayEntity(r);
            }
#else
            throw new NotImplementedException("SPs work fine with CipherDB but currently demonstrated only in the EntityFramework Database First sample");
#endif
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
            using (var secDb = new SecDbContext())
            {
                var creditCardSearchPrefix = SecureSearch.GetPrefix(ccSearchStr);
                // This query will take place on encrypted data at the database side
                var usersFound = secDb.Users.Where(u => u.SecureSearch_CreditCardNumber.StartsWith(creditCardSearchPrefix));
                if (usersFound == null || usersFound.Count() < 1)
                    Console.WriteLine("No results found");
                foreach (var user in usersFound)
                {
                    DisplayEntity(user);
                }
            }
        }

        public class ProjectedUser
        {
            [Secure]
            public string Name { get; set; }

            [Secure]
            public string CreditCard { get; set; }

            [Secure]
            public byte[] SSN { get; set; }

            public string OrderItem { get; set; }
        }

        public void Test()
        {
            // Placeholder for anything you want ...
        }

        private int ReadAll(bool printToScreen)
        {
            int totalEntries = 0;
            using (var secDb = new SecDbContext())
            {
                foreach (var o in secDb.Users)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o);
                }
            }
            return totalEntries;
        }

        private int ReadAllInsecure(bool printToScreen)
        {
            Console.WriteLine("This skips Crypteron decryption, so data is unreadable by anyone");
            int totalEntries = 0;
            using (var plainDbContext = new PlainDbContext())
            {
                foreach (var o in plainDbContext.Users)
                {
                    totalEntries++;
                    if (printToScreen)
                        DisplayEntity(o, true);
                }
            }
            return totalEntries;
        }

        private void DisplayEntity(User o, bool rawBits = false)
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

        private User CreateRandomUser()
        {
            var rndUser = new User
            {
                OrderId = 0, //db overwrites this
                OrderItem = UserRandomizer.GetRandomItem(),
                CustomerName = UserRandomizer.GetRandomNames(),
                Timestamp = Randomizer.GetRandomTime(),
                SecureSearch_CreditCardNumber = UserRandomizer.GetRandomCC(),
                Secure_LegacyPIN = UserRandomizer.GetRandomPIN(),
                Secure_SocialSecurityNumber = Encoding.UTF8.GetBytes(UserRandomizer.GetRandomSSN())
            };
            return rndUser;
        }

        private User CreateOrEditUser(User editUser = null)
        {
            User usr;
            if (editUser == null)
            {
                usr = new User
                    {
                        OrderId = 0
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

        private User SanitizeUserProperties(User input)
        {
            input.CustomerName = LimitString(input.CustomerName, 64);
            input.OrderItem = LimitString(input.OrderItem, 64);
            input.SecureSearch_CreditCardNumber = LimitString(input.SecureSearch_CreditCardNumber, 64);
            input.Secure_LegacyPIN = LimitString(input.Secure_LegacyPIN, 64);

            string temp;
            if (input.Secure_SocialSecurityNumber == null)
                temp = String.Empty;
            else
                temp = Encoding.UTF8.GetString(input.Secure_SocialSecurityNumber);
            
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
            // Maximum: ORM guidelines suggest breaking down into batches of a few hundred entities
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
            { 
                // This is the usual case, sometimes even negative on powerful systems with h/w encryption support (AES-NI)
                // since encryption piece is negligible and other processing dominates timing (!)
                Console.WriteLine("Read impact of entire security processing pipeline is less than a single millisecond on average!");
            }
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
