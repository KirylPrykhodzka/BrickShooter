﻿using BrickShooter.Collision;
using BrickShooter.Constants;
using BrickShooter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BrickShooter.GameObjects
{
    public class Wall : ICollisionActor
    {
        public Texture2D Texture { get; set; }
        public Rectangle RectBounds { get; set; }
        public ColliderPolygon ColliderBounds { get; }

        public Wall(Texture2D texture, Rectangle rectBounds)
        {
            Texture = texture;
            RectBounds = rectBounds;
            ColliderBounds = new ColliderPolygon();
            ColliderBounds.Points.Add(new Vector2(RectBounds.X, RectBounds.Y));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X + RectBounds.Width, RectBounds.Y));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X + RectBounds.Width, RectBounds.Y + RectBounds.Height));
            ColliderBounds.Points.Add(new Vector2(RectBounds.X, RectBounds.Y + RectBounds.Height));
            ColliderBounds.BuildEdges();
        }

        public void Draw()
        {
            VisualizationHelper.VisualizeCollider(ColliderBounds.Points);

            GlobalObjects.SpriteBatch.Draw(
                Texture,
                RectBounds,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                Layers.WALLS);
        }
    }
}
