using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace Match3Mono
{
    public class Gem
    {
        public bool DebugInfoEnabled = false;

        public const int GemSize = 64;

        // Callbacks
        private readonly Action onAnimationCompleted;
        private Action onMatchCompleted;

        // Events control
        private bool animating = false;
        private bool selected = false;
        private bool matching = false;
        private bool queuedForMatch = false;
        private int spriteIndex = -1;

        // Translation control
        private float moveToX = 0;
        private float moveToY = 0;

        // Rotation Control
        private float rotationDeg = 0f;
        private float rotationDirection = 1;

        // Rendering attributes
        private float rotationRad = 0f;
        private Color color = Color.White;
        private readonly Texture2D texture = null;

        public Vector2 position;

        // Constants
        private const int TRANSLATE_SPEED = 400;
        private const float MAX_ROTATION_DEG = 15;
        private const float DESTROY_SPEED = 1.3f;
        private const int ROTATION_SPEED = 45;

        private float sizeFactor = 1f;

        public Gem(int GemIndex, Texture2D texture, Vector2 position, Action onAnimationCompleted)
        {
            this.spriteIndex = GemIndex;
            this.texture = texture;
            this.onAnimationCompleted = onAnimationCompleted;
            this.position = position;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            _spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)((position.X + GemSize / 2) * sizeFactor),
                    (int)((position.Y + GemSize / 2) * sizeFactor),
                    (int)(GemSize * sizeFactor),
                    (int)(GemSize * sizeFactor)
                ),
                null,
                selected ? Color.HotPink : color,
                rotationRad,
                origin,
                SpriteEffects.None,
                0f
            );

            if (DebugInfoEnabled)
            {
                _spriteBatch.DrawString(
                    AssetsLoader.GetInstance().GetFont("fontSm"),
                    (animating ? "Anim" : "") + (matching ? "Match" : ""),
                    new Vector2(position.X, position.Y + 20),
                    Color.Magenta
                );
            }

            // Reset after render
            rotationRad = 0f;
        }

        public void Update(GameTime gameTime)
        {
            this.UpdatedSelected((float)gameTime.ElapsedGameTime.TotalSeconds);
            this.UpdateTranslate((float)gameTime.ElapsedGameTime.TotalSeconds);
            this.UpdateMatchingProcess((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public Vector2 GetDestiny()
        {
            if (moveToX > 0 || moveToY > 0)
            {
                return new Vector2(moveToX, moveToY);
            }
            return position;
        }

        public void SetSelected(bool nextSelect)
        {
            this.selected = nextSelect;
        }

        public void MoveTo(Vector2 nextPosition)
        {
            this.animating = true;
            this.moveToX = nextPosition.X;
            this.moveToY = nextPosition.Y;
        }

        public void Match()
        {
            this.animating = true;
            this.matching = true;
        }

        public int SpriteIndex => this.spriteIndex;

        public void QueueForMatch()
        {
            queuedForMatch = true;
        }

        public bool IsQueuedForMatch => queuedForMatch;

        private void UpdatedSelected(float delta)
        {
            if (this.selected)
            {
                if (rotationDeg < MAX_ROTATION_DEG * -1 || rotationDeg > MAX_ROTATION_DEG)
                {
                    rotationDirection *= -1;
                }
                rotationDeg += rotationDirection * delta;
                rotationRad = MathHelper.ToRadians(rotationDeg);
            }
            else
            {
                rotationRad = MathHelper.ToRadians(rotationDeg * -1);
                rotationDirection = ROTATION_SPEED;
                rotationDeg = 0;
            }
        }

        private void UpdateTranslate(float delta)
        {
            float step = delta * TRANSLATE_SPEED;

            float diffX = moveToX != 0 ? moveToX - position.X : 0f;
            float diffY = moveToY != 0 ? moveToY - position.Y : 0f;

            if (Math.Abs(diffX) > 5)
            {
                position.X += diffX > 0 ? step : step * -1;
            }
            else
            {
                moveToX = 0;
            }

            if (Math.Abs(diffY) > 5)
            {
                position.Y += diffY > 0 ? step : step * -1;
            }
            else
            {
                moveToY = 0;
            }

            if (moveToX == 0 && moveToY == 0 && animating && !matching)
            {
                animating = false;
                selected = false;
                onAnimationCompleted?.Invoke();
            }
        }

        private void UpdateMatchingProcess(float delta)
        {
            if (!matching)
            {
                return;
            }

            float step = delta * DESTROY_SPEED;

            sizeFactor -= step;

            if (DebugInfoEnabled)
            {
                Debug.WriteLine(sizeFactor.ToString("0.000"));
            }

            if (sizeFactor < 0.1)
            {
                OnMatchDone();
            }
        }

        private void OnMatchDone()
        {
            if (animating)
            {
                onAnimationCompleted?.Invoke();
            }
            matching = false;
            spriteIndex = -1;
            animating = false;
            onMatchCompleted?.Invoke();
        }
    }
}
