using Crypteron.CipherObject;
using Crypteron.SampleApps.CommonCode;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ConsoleCipherObject.EFCore
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static string DbConnectionString { get; set; }

        private static void Init()
        {
            var currDir = Directory.GetCurrentDirectory();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(currDir)
                .AddJsonFile("appsettings.json")
                .Build();

            DbConnectionString = Configuration["ConnectionStrings:SqlDatabase"];

            // This is just to get the connection string to the right location
            if (DbConnectionString.Contains("%CONTENTROOTPATH%"))
            {
                DbConnectionString = DbConnectionString.Replace("%CONTENTROOTPATH%", currDir);
            }

            Crypteron.CrypteronConfig.Config.MyCrypteronAccount.AppSecret = Configuration["CrypteronConfig:MyCrypteronAccount:AppSecret"];

            // Showing additional Crypteron settings, adapt to your situation.
            // Settings explained at https://www.crypteron.com/developers-guide/cipherdb/dotnet/#Configuration_settings
            // Uncomment in appsettings.json and just below
            //Crypteron.CrypteronConfig.Config.CommonConfig.AllowNullsInSecureFields =
            //   bool.Parse(Configuration["CrypteronConfig:CommonConfig:AllowNullsInSecureFields"]);
        }

        public static void Main()
        {
            // Initialize EF connection string and Crypteron's AppSecret
            Init();

            using (var db = new AppContext())
            {
                // Create new user
                var newRandomUser = new User
                {
                    FullName = UserRandomizer.GetRandomNames(), // "John Doe"
                    SocialSecurityNumber = UserRandomizer.GetRandomSSN(), // "123-45-6789"
                    CreditCardNumber = UserRandomizer.GetRandomCC() // "1234 5678 9012 3456"
                };
                // Encrypt our new user
                newRandomUser.Seal();
                db.Users.Add(newRandomUser);

                // Save to database
                var count = db.SaveChanges();
                Console.WriteLine("{0} users saved to database", count);

                Console.WriteLine();
                Console.WriteLine("All users in database:");
                foreach (var user in db.Users)
                {
                    // Encrypted version read as-is
                    Console.WriteLine($"Encrypted: {user.UserId}: {user.FullName}, CC={user.CreditCardNumber}, SSN={user.SocialSecurityNumber}");

                    // Decrypt user
                    user.Unseal();

                    // Do work as needed, we just print to console
                    Console.WriteLine($"Decrypted: {user.UserId}: {user.FullName}, CC={user.CreditCardNumber}, SSN={user.SocialSecurityNumber}");
                }
            }

            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
        }
    }
}
