﻿using System.Collections.Generic;
using BrickShooter.Physics.Interfaces;
using BrickShooter.Physics.Models;

namespace BrickShooter.Physics
{
    public class CollisionProcessor : ICollisionProcessor
    {
        public void ProcessFutureCollisions(MaterialObject currentObject, IReadOnlyCollection<CollisionCalculationResult> futureCollisions)
        {
            //if we know that player will collide with a wall, and we know the distance to collision,
            //we place player as close to the wall as we can without clipping them inside
            //if we know the bullet will collide with a wall, we place it near the wall and bounce (change velocity)
            //if a bullet will collide with a player, we can process it in a special way by stopping the game
            throw new System.NotImplementedException();
        }
    }
}
