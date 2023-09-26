using System.Collections.Generic;

namespace BrickShooter.Drawing
{
    public static class DrawingSystem
    {
        private static readonly HashSet<IDrawableObject> drawableObjects = new();

        public static void Register(IDrawableObject drawable)
        {
            drawableObjects.Add(drawable);
        }

        public static void Unregister(IDrawableObject drawable)
        {
            drawableObjects.Remove(drawable);
        }

        //should be called on each level reload
        public static void Reset()
        {
            drawableObjects.Clear();
        }

        public static void Run()
        {
            foreach(var drawableObject in drawableObjects)
            {
                drawableObject.Draw();
            }
        }
    }
}
