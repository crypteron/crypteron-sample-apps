using System;
using Crypteron;
using Newtonsoft.Json;
using Crypteron.CipherObject;

namespace Crypteron.SampleApps.ConsoleCipherObject
{
    internal class Program
    {
        class Patient
        {
            public int Id { get; set; }

            [Secure]
            public string Name { get; set; }

            [Secure]
            public string SocialSecurityNumber { get; set; }
        }

        private static void Main(string[] args)
        {
            // Code is intentionally verbose for step-by-step guidance

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Encrypting/Sealing an object");
            Console.WriteLine("----------------------------------------------");
            var patient = new Patient { Name = "John Doe", SocialSecurityNumber = "555-55-5555" };
            Console.WriteLine($"  Unprotected Object: {JsonConvert.SerializeObject(patient)}" + Environment.NewLine);
            Console.WriteLine(" => Seal() it" + Environment.NewLine);
            patient.Seal();  // extension method or by Crypteron.CipherObject.Seal(patient) are identical
            Console.WriteLine($"  Sealed Object: {JsonConvert.SerializeObject(patient)}" + Environment.NewLine);

            YourMethodToWriteToDatabase(patient);

            Console.WriteLine(" => Now write data to any network, storage, 3rd party systems etc. Data is always encrypted and protected." + Environment.NewLine);


            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Decrypting/Unsealing an object");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" => Read encrypted object from any network, storage, 3rd party systems etc." + Environment.NewLine);

            var patient2 = YourMethodToReadFromDatabase();

            Console.WriteLine($"  Sealed Object: {JsonConvert.SerializeObject(patient2)}" + Environment.NewLine);
            Console.WriteLine(" => Unseal() it " + Environment.NewLine);
            patient2.Unseal(); // extension method or by Crypteron.CipherObject.Unseal(patient2) are identical
            Console.WriteLine($"  Clear Object : {JsonConvert.SerializeObject(patient2)}" + Environment.NewLine);
            Console.WriteLine(" => You can now process this object as desired " + Environment.NewLine);
            Console.WriteLine("Additional documentation at http://crypteron.com/docs or contact support@crypteron.com for assistance" + Environment.NewLine);
            Console.WriteLine("Press enter to exit ... ");
            Console.ReadLine();
        }

        private static void YourMethodToWriteToDatabase(Patient patient)
        {
            // Write as you would to any database, SQL, NoSQL or even message queues or REST endpoints
        }

        private static Patient YourMethodToReadFromDatabase()
        {
            // Read as you would from any database, SQL, NoSQL or even message queues or REST endpoints

            // Simulating that here ..
            var dbRead = new Patient
            {
                Name = "Jack Reacher",
                SocialSecurityNumber = "777-77-7777"
            };
            // Since we write sealed, we would read sealed too, simulate that too
            return dbRead.Seal();
        }
    }
}
