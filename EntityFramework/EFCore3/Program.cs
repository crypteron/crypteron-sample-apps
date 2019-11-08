using Crypteron.SampleApps.CommonCode;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Crypteron.SampleApps.EFCore3
{
    public class Program
    {
        public static string DatabaseConnectionString { get; set; }

        private static void ConfigureSettings()
        {
            var currDir = Directory.GetCurrentDirectory();
            var Configuration = new ConfigurationBuilder()
                .SetBasePath(currDir)
                .AddJsonFile("appsettings.json")
                .Build();

            DatabaseConnectionString = Configuration["ConnectionStrings:DefaultConnection"];

            // This is just to get the connection string to the right location
            if (DatabaseConnectionString.Contains("%CONTENTROOTPATH%"))
            {
                DatabaseConnectionString = DatabaseConnectionString.Replace("%CONTENTROOTPATH%", currDir);
            }

            CrypteronConfig.Config.MyCrypteronAccount.AppSecret = Configuration["CrypteronConfig:MyCrypteronAccount:AppSecret"];
        }

        public static void Main()
        {
            ConfigureSettings();

            using (var db = new AppDbContext())
            {
                var newUser = GenerateRandomUser();
                Console.WriteLine("Adding user to database ...");
                Console.WriteLine("---------------------------");

                db.Users.Add(newUser);
                db.SaveChanges();

                PrintUser(newUser);
                Console.WriteLine();
                Console.WriteLine("Decrypted user : read from database (access thru Entity Framework + Crypteron CipherDb) ... ");
                Console.WriteLine("--------------------------------------------------------------------------------------------");

                foreach (var user in db.Users)
                {
                    PrintUser(user);
                }
                Console.WriteLine();
            }

            // Directly read from database bypassing EF Core + CipherDb
            using (var connection = new SqliteConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand($"SELECT * FROM [Users]", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Encrypted user : access without Crypteron ... ");
                        Console.WriteLine("----------------------------------------------");
                        var user = new User
                        {
                            UserId = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            CreditCardNumber = reader.GetString(2),
                            SocialSecurityNumber = reader.GetString(3),
                            FacePhoto = reader.GetByteArray(4)
                        };
                        PrintUser(user);
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("You can also open the SQLite *.db file in the \\bin\\Debug\\netcoreapp2.1\\ folder to verify data-at-rest encryption ...");
            Console.WriteLine();

            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
        }

        public static User GenerateRandomUser()
        {
            // Create new user, random test data
            return new User
            {
                FullName = UserRandomizer.GetRandomNames(), // "John Doe"
                SocialSecurityNumber = UserRandomizer.GetRandomSSN(), // "123-45-6789"
                CreditCardNumber = UserRandomizer.GetRandomCC(), //  "1234 5678 9012 3456"
                FacePhoto = UserRandomizer.GetRandomBytes() // 0x123456789 ..
            };
        }

        private static void PrintUser(User user)
        {
            Console.WriteLine(JsonConvert.SerializeObject(user, Formatting.Indented));
        }
    }

    public static class SqliteExtensions
    {
        public static byte[] GetByteArray(this SqliteDataReader reader, int i)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (var stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(i, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
    }
}
