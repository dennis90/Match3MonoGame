using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3Mono
{
    public abstract class Scene
    {
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
        public abstract void Destroy();
        public abstract void ToggleDebug();
    }
}
