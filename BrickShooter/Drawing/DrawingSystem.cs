using System.Collections.Generic;

namespace BrickShooter.Drawing
{
    public class DrawingSystem : IDrawingSystem
    {
        private readonly HashSet<IDrawableObject> drawableObjects = new();

        public void Register(IDrawableObject drawable)
        {
            drawableObjects.Add(drawable);
        }

        public void Unregister(IDrawableObject drawable)
        {
            drawableObjects.Remove(drawable);
        }

        //should be called on each level reload
        public void Reset()
        {
            drawableObjects.Clear();
        }

        public void Run()
        {
            foreach(var drawableObject in drawableObjects)
            {
                drawableObject.Draw();
            }
        }
    }
}
