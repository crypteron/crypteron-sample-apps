using Crypteron.SampleApps.CommonCode;

namespace Crypteron.SampleApps.ConsoleCipherDbNh4
{
    class Program
    {
        private static void Main(string[] args)
        {
            // http://dotnetanalysis.blogspot.com/2012/10/nhibernate-tutorial-for-beginners-with.html
            // http://nhforge.org/wikis/howtonh/your-first-nhibernate-based-application.aspx
            var app = new App();
            app.Run();
        }
    }
}
