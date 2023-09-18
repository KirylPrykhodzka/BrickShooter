﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BrickShooter.Collision
{
    public class ColliderPolygon
    {
        private readonly List<Vector2> points = new();
        private readonly List<Vector2> edges = new();

        public void BuildEdges()
        {
            Vector2 p1;
            Vector2 p2;
            edges.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                edges.Add(p2 - p1);
            }
        }

        public List<Vector2> Edges
        {
            get { return edges; }
        }

        public List<Vector2> Points
        {
            get { return points; }
        }

        public Vector2 Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    totalX += points[i].X;
                    totalY += points[i].Y;
                }

                return new Vector2(totalX / (float)points.Count, totalY / (float)points.Count);
            }
        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p = points[i];
                points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }
    }
}
