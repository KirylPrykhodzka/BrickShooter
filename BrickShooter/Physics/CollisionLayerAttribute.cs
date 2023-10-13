using System;

namespace BrickShooter.Physics
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CollisionLayerAttribute : Attribute
    {
        public string LayerName { get; }

        public CollisionLayerAttribute(string layerName)
        {
            LayerName = layerName;
        }
    }
}