using Crypteron.SampleApps.CommonCode;

namespace Crypteron.SampleApps.EF6.CodeFirst.PostgreSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Can be in the .config file OR via code as demo'd here. Configuration via code allows for more
            // complex deployments as the AppSecret can be pluggin in via any arbitrary process
            CrypteronConfig.Config.MyCrypteronAccount.AppSecret = "Replace_this_with_app_secret_from_https://my.crypteron.com";

            var app = new App();
            app.Run();
        }
    }
}
