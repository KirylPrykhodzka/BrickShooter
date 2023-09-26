using BrickShooter.Physics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;

namespace BrickShooter.Configuration
{
    public class Startup
    {
        public Startup(HostingEnvironment env) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPhysicsSystem, PhysicsSystem>();
        }
    }
}
