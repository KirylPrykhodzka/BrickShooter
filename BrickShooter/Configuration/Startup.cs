using BrickShooter.Drawing;
using BrickShooter.GameObjects;
using BrickShooter.Physics;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Resources;
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
            services.AddSingleton<IDrawingSystem, DrawingSystem>();
            services.AddSingleton<IPotentialCollisionsDetector, PotentialCollisionsDetector>();
            services.AddSingleton<IExistingCollisionsCalculator, ExistingCollisionsCalculator>();
            services.AddSingleton<IFutureCollisionsCalculator, FutureCollisionsCalculator>();
            services.AddSingleton<IMaterialObjectMover, MaterialObjectMover>();

            //object pools
            services.AddSingleton<IPool<Bullet>, Pool<Bullet>>();
        }
    }
}
