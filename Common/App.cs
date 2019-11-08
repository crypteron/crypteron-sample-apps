using System;

// This way to minimize code duplication
#if EF6CODEFIRST
using Crypteron.SampleApps.EF6.CodeFirst;
#endif

#if EF6DBFIRST
using Crypteron.SampleApps.EF6.DbFirst;
#endif

namespace Crypteron.SampleApps.CommonCode
{
    public class App
    {
        public void Run()
        {
            var cust = new ProcessCustomer();
            var ongoing = true;

            while (ongoing)
            {
                PrintMenu();
                // Get char in lower case, skipping whitespaced chars
                var userInput = Console.ReadLine().ToLowerInvariant().Trim();
                Console.WriteLine(" ...");
                switch (userInput)
                {
                    case "1":
                        cust.ReadAll();
                        break;
                    case "2":
                        cust.Create();
                        break;
                    case "3":
                        cust.CreateAuto(1);
                        break;
                    case "4":
                        cust.CreateAuto(10);
                        break;
                    case "5":
                        cust.Update();
                        break;
                    case "6":
                        cust.Delete();
                        break;
                    case "7":
                        cust.EncryptedSearchServerSide();
                        break;
                    case "8":
                        cust.WipeAllViaSql();
                        break;
                    case "9":
                        cust.ReadAllInsecure();
                        break;
                    case "10":
                        cust.Benchmark();
                        break;
                    case "11":
                        cust.StoredProcedure();
                        break;
                    case "t":
                        cust.Test();
                        break;
                    case "q":
                        ongoing = false;
                        break;
                    default:
                        Console.WriteLine("Unknown input");
                        break;
                }
            }
        }

        public void SingleRun()
        {
            var cust = new ProcessCustomer();
            cust.Test();
        }

        private void PrintMenu()
        {
            Console.WriteLine("Crypteron Sample App: Make your selection ...");
            Console.WriteLine("====================================================");
            Console.WriteLine(" 1 : Read all customer orders in table");
            Console.WriteLine(" 2 : Create a new customer order manually via the keyboard");
            Console.WriteLine(" 3 : Auto-create a new customer order");
            Console.WriteLine(" 4 : Auto-create 10 customer orders");
            Console.WriteLine(" 5 : Update an existing customer order");
            Console.WriteLine(" 6 : Delete a particular customer order in table");
            Console.WriteLine(" 7 : Search encrypted data (server side)");
            Console.WriteLine(" 8 : Wipe all customer orders via direct SQL");
            Console.WriteLine(" 9 : Read without Crypteron (data is undecipherable)");
            Console.WriteLine("10 : Benchmark with/without encryption");
            Console.WriteLine("11 : Run a stored procedure via CipherDB");
            //Console.WriteLine("T : Test");
            Console.WriteLine(" Q : Quit");

            Console.Write("\nChoose and hit enter: ");
        }
    }
}
