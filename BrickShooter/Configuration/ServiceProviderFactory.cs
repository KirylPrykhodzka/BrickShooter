using System.IO;
using System;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BrickShooter.Configuration
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; set; }

        static ServiceProviderFactory()
        {
            HostingEnvironment env = new();
            env.ContentRootPath = Directory.GetCurrentDirectory();
            env.EnvironmentName = "Development";

            Startup startup = new(env);
            ServiceCollection sc = new();
            startup.ConfigureServices(sc);
            ServiceProvider = sc.BuildServiceProvider();
        }
    }
}
