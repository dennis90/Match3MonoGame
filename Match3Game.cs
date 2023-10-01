using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Match3Mono
{
    public class Match3Game : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D[] piecesTextures;
        private GameBoard _board;

        private SpriteFont font;
        private Texture2D background;

        const int ResolutionWidth = 1280;
        const int ResolutionHeight = 768;

        public Match3Game()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ResolutionWidth,
                PreferredBackBufferHeight = ResolutionHeight
            };
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            piecesTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("gems/black"),
                Content.Load<Texture2D>("gems/blue"),
                Content.Load<Texture2D>("gems/green"),
                Content.Load<Texture2D>("gems/grey"),
                Content.Load<Texture2D>("gems/orange"),
                Content.Load<Texture2D>("gems/pink"),
                Content.Load<Texture2D>("gems/red"),
                Content.Load<Texture2D>("gems/yellow"),
            };

            background = Content.Load<Texture2D>("sky");
            font = Content.Load<SpriteFont>("galleryFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                _board = new GameBoard(piecesTextures, font);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F3) && _board != null)
            {
                _board.DebugInfoEnabled = !_board.DebugInfoEnabled;
            }

            _board?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                background,
                new Rectangle(0, 0, ResolutionWidth, ResolutionHeight),
                Color.White
            );

            _board?.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
