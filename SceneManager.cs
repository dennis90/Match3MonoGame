using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3Mono
{
    internal class SceneManager
    {
        private static SceneManager _sceneManager;

        private Scene _currentScene;

        private SceneManager()
        {
            // Default scene
            _currentScene = new MainMenu();
        }

        public static SceneManager GetInstance()
        {
            if (_sceneManager == null)
            {
                _sceneManager = new SceneManager();
            }

            return _sceneManager;
        }

        public void SetScene(Scene nextScene)
        {
            _currentScene.Destroy();

            _currentScene = nextScene;
        }

        public void Update(GameTime gameTime)
        {
            _currentScene.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentScene.Draw(spriteBatch);
        }

        public void ToggleDebug()
        {
            _currentScene.ToggleDebug();
        }
    }
}
