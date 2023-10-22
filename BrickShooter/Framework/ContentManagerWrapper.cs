using Microsoft.Xna.Framework.Content;

namespace BrickShooter.Framework
{
    public class ContentManagerWrapper : IContentManager
    {
        private readonly ContentManager contentManager;

        public ContentManagerWrapper(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public T Load<T>(string path)
        {
            return contentManager.Load<T>(path);
        }
    }
}
