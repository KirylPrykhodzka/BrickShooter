﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Physics.Interfaces
{
    public interface IMaterialObject
    {
        Vector2 Position { get; set; }
        float Rotation { get; }
        Vector2 Velocity { get; set; }
        IList<IColliderPolygon> Colliders { get; }
        bool DidRotate { get; }
    }
}
