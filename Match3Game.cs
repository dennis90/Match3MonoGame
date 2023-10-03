using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Match3Mono
{
    public class Match3Game : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private bool initialized = false;

        private readonly AssetsLoader assetsStore;

        public const int ResolutionWidth = 1280;
        public const int ResolutionHeight = 768;
        private SceneManager sceneManager;

        public Match3Game()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ResolutionWidth,
                PreferredBackBufferHeight = ResolutionHeight
            };
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            assetsStore = AssetsLoader.GetInstance(Content.ServiceProvider, Content.RootDirectory);

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            assetsStore.LoadTextureList(
                "gems",
                new string[]
                {
                    "gems/black",
                    "gems/blue",
                    "gems/green",
                    "gems/grey",
                    "gems/orange",
                    "gems/pink",
                    "gems/red",
                    "gems/yellow",
                }
            );

            assetsStore.LoadTexture("ui/background");
            assetsStore.LoadTexture("ui/button");

            assetsStore.LoadFont("fontSm");
            assetsStore.LoadFont("fontLg");
            assetsStore.LoadFont("fontXl");

            assetsStore.LoadSoundEffect("sound/Menu");
            assetsStore.LoadSoundEffect("sound/Level1");
            assetsStore.LoadSoundEffect("sound/Level2");
            assetsStore.LoadSoundEffect("sound/Level3");
            assetsStore.LoadSoundEffect("sound/Credits");

            assetsStore.LoadSoundEffect("sound/BadMove");
            assetsStore.LoadSoundEffectList(
                "sound/Swap",
                new string[] { "sound/Swap00", "sound/Swap01", "sound/Swap02", "sound/Swap03", }
            );

            initialized = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            if (initialized && sceneManager == null)
            {
                sceneManager = SceneManager.GetInstance();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F2) && sceneManager != null)
            {
                sceneManager.SetScene(new GameOver());
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3) && sceneManager != null)
            {
                sceneManager.ToggleDebug();
            }

            sceneManager?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                assetsStore.GetTexture("ui/background"),
                new Rectangle(0, 0, ResolutionWidth, ResolutionHeight),
                Color.White
            );

            sceneManager?.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
