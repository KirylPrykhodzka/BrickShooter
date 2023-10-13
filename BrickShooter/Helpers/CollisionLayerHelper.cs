using BrickShooter.Physics;
using System;
using System.Linq;

namespace BrickShooter.Helpers
{
    public static class CollisionLayerHelper
    {
        public static string GetCollisionLayer(object obj)
        {
            Type objectType = obj.GetType();

            if (objectType
                    .GetCustomAttributes(typeof(CollisionLayerAttribute), true)
                    .FirstOrDefault() is CollisionLayerAttribute attribute)
            {
                return attribute.LayerName;
            }

            return string.Empty;
        }
    }
}
