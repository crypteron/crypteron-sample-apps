using System;
using Newtonsoft.Json;
using Crypteron.CipherObject;

namespace Crypteron.SampleApps.CipherObject
{
    public class Program
    {
        class Patient
        {
            public int Id { get; set; }

            [Secure]
            public string Name { get; set; }

            [Secure]
            public string SocialSecurityNumber { get; set; }
        }

        public static void Main(string[] args)
        {
            // Code is intentionally verbose for step-by-step guidance
            var patient = new Patient { Name = "John Doe", SocialSecurityNumber = "555-55-5555" };
            Console.WriteLine("Object");
            Console.WriteLine("------");
            DisplayObject(patient);

            Console.WriteLine("Encrypted");
            Console.WriteLine("---------");
            patient.Encrypt();  // ... or Crypteron.CipherObject.Encrypt(patient), both are identical
            DisplayObject(patient);
            
            Console.WriteLine("Decrypted");
            Console.WriteLine("---------");
            patient.Decrypt(); // extension method or by Crypteron.CipherObject.Decrypt(patient2) are identical
            DisplayObject(patient);

            Console.WriteLine(Environment.NewLine + "Additional documentation at https://www.crypteron.com/docs or contact support(at)crypteron.com for assistance" + Environment.NewLine);
            Console.WriteLine("Press enter to exit ... ");
            Console.ReadLine();
        }

        private static void DisplayObject(Patient patient)
        {
            Console.WriteLine(JsonConvert.SerializeObject(patient, Formatting.Indented));
        }
    }
}
