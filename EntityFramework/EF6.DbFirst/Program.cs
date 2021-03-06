﻿using Crypteron.SampleApps.CommonCode;

namespace Crypteron.SampleApps.EF6.DbFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTE: The `dotnet` CLI and new csproj format CANNOT embed EF6 database-first's EDMX
            //       resources into build. So this project is preserved in the earlier csproj format
            //       till https://github.com/dotnet/cli/issues/8193 is resolved

            var app = new App();
            app.Run();
        }
    }
}
