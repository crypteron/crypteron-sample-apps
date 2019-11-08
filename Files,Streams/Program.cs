using System;

namespace Crypteron.SampleApps.CipherStor
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();
                app.Run().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine($">> Unexpected failure: {e}" + Environment.NewLine + "Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
