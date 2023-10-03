using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Match3Mono
{
    public class MainMenu : Scene
    {
        private bool DebugEnabled = false;
        private bool hovered = false;
        private bool clicked = false;
        private readonly AssetsLoader assetsStore;
        private SoundEffectInstance music;

        public MainMenu()
        {
            assetsStore = AssetsLoader.GetInstance();
            music = assetsStore.GetSoundEffect("sound/Menu").CreateInstance();
            music.IsLooped = true;
            music.Volume = .3f;
            music.Play();
        }

        public override void Destroy()
        {
            music.Stop();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string text = "New Game";
            var font = AssetsLoader.GetInstance().GetFont("fontLg");
            var origin = Vector2.Divide(font.MeasureString(text), 2);

            if (hovered)
            {
                Mouse.SetCursor(MouseCursor.Hand);
            }
            else
            {
                Mouse.SetCursor(MouseCursor.Arrow);
            }

            spriteBatch.Draw(
                assetsStore.GetTexture("ui/button"),
                new Rectangle(
                    Match3Game.ResolutionWidth / 2 - 380 / 2,
                    Match3Game.ResolutionHeight / 2 - 98 / 2,
                    380,
                    98
                ),
                hovered ? Color.Purple : Color.White
            );
            spriteBatch.DrawString(
                font,
                text,
                new Vector2(Match3Game.ResolutionWidth / 2, Match3Game.ResolutionHeight / 2),
                hovered ? Color.White : Color.Purple,
                0f,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public override void ToggleDebug()
        {
            DebugEnabled = !DebugEnabled;
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;

            if (
                mousePos.X > (Match3Game.ResolutionWidth / 2 - 380 / 2)
                && mousePos.X < (Match3Game.ResolutionWidth / 2 + 380 / 2)
                && mousePos.Y > (Match3Game.ResolutionHeight / 2 - 98 / 2)
                && mousePos.Y < (Match3Game.ResolutionHeight / 2 + 98 / 2)
            )
            {
                hovered = true;
            }
            else
            {
                hovered = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                clicked = true;
            }

            if (mouseState.LeftButton == ButtonState.Released && clicked)
            {
                clicked = false;

                if (hovered)
                {
                    SceneManager.GetInstance().SetScene(new GameBoard());
                }
            }
        }
    }
}
