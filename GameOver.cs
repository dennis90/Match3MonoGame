using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Match3Mono
{
    public class GameOver : Scene
    {
        private readonly SoundEffectInstance music;
        private readonly Rectangle buttonBounds =
            new(
                Match3Game.ResolutionWidth / 2 - 380 / 2,
                Match3Game.ResolutionHeight / 2 - 98 / 2 + 100,
                380,
                98
            );

        private bool hovered = false;

        public GameOver()
        {
            music = AssetsLoader.GetInstance().GetSoundEffect("sound/Credits").CreateInstance();
            music.Volume = .3f;
            music.Play();
        }

        public override void Destroy()
        {
            music?.Stop();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var fontLg = AssetsLoader.GetInstance().GetFont("fontLg");
            var fontXl = AssetsLoader.GetInstance().GetFont("fontXl");

            string gameOverTxt = "Game Over!";
            var gameOverOrigin = Vector2.Divide(fontXl.MeasureString(gameOverTxt), 2);

            spriteBatch.DrawString(
                fontXl,
                gameOverTxt,
                new Vector2(
                    Match3Game.ResolutionWidth / 2 + 1,
                    Match3Game.ResolutionHeight / 2 + 1
                ),
                Color.Black,
                0f,
                gameOverOrigin,
                1f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString(
                fontXl,
                gameOverTxt,
                new Vector2(Match3Game.ResolutionWidth / 2, Match3Game.ResolutionHeight / 2),
                Color.White,
                0f,
                gameOverOrigin,
                1f,
                SpriteEffects.None,
                0f
            );

            string newGameTxt = "New Game?";
            var newGameOrigin = Vector2.Divide(fontLg.MeasureString(newGameTxt), 2);

            if (hovered)
            {
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }

            spriteBatch.Draw(
                AssetsLoader.GetInstance().GetTexture("ui/button"),
                buttonBounds,
                hovered ? Color.Purple : Color.White
            );

            spriteBatch.DrawString(
                fontLg,
                newGameTxt,
                new Vector2(Match3Game.ResolutionWidth / 2, Match3Game.ResolutionHeight / 2 + 100),
                hovered ? Color.White : Color.Purple,
                0f,
                newGameOrigin,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public override void ToggleDebug()
        {
            // It does nothing
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();

            if (
                mState.X > buttonBounds.Left
                && mState.X < buttonBounds.Right
                && mState.Y > buttonBounds.Top
                && mState.Y < buttonBounds.Bottom
            )
            {
                hovered = true;
            }
            else
            {
                hovered = false;
            }
        }
    }
}
