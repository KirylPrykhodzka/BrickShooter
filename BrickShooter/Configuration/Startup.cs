using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
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
            services.AddSingleton<IPotentialCollisionsDetector, PotentialCollisionsDetector>();
            services.AddSingleton<ICollisionCalculator, CollisionCalculator>();
            services.AddSingleton<ICollisionProcessor, CollisionProcessor>();
            services.AddSingleton<IMaterialObjectMover, MaterialObjectMover>();
        }
    }
}
