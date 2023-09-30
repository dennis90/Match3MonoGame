using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Match3Mono
{
    public class GameBoard
    {
        const int BoardWidth = 8;
        const int BoardHeight = 8;
        const int Padding = 10;
        const int MarginX = 20;
        const int MarginY = 20;

        private readonly Gem[][] state;
        private readonly AnimationManager animationManager = new();
        private readonly Random rand = new();
        private readonly Texture2D[] textureList;

        private bool firstClick = false;
        Point? firstGem = null;

        public GameBoard(Texture2D[] _textureList)
        {
            this.textureList = _textureList;
            state = new Gem[BoardWidth][];

            for (int x = 0; x < BoardWidth; x++)
            {
                state[x] = new Gem[BoardHeight];

                for (int y = 0; y < BoardHeight; y++)
                {
                    int textureIndex = rand.Next(0, textureList.Length);
                    state[x][y] = new Gem(
                        textureIndex,
                        textureList[textureIndex],
                        new Vector2(
                            MarginX + (x * (Gem.GemSize + Padding)),
                            MarginY + (y * (Gem.GemSize + Padding))
                        ),
                        animationManager.OnAnimationCompleted
                    );
                }
            }
        }

        private void RemoveGemAt(Point point)
        {
            state[point.X][point.Y] = null;
        }

        private static Point PositionToGemIndex(Vector2 coord)
        {
            if (
                coord.X >= MarginX
                && coord.Y >= MarginY
                && coord.X <= MarginX + ((Gem.GemSize + Padding) * BoardWidth)
                && coord.Y <= MarginY + ((Gem.GemSize + Padding) * BoardHeight)
            )
            {
                return new Point(
                    ((int)coord.X - MarginX) / (Gem.GemSize + Padding),
                    ((int)coord.Y - MarginY) / (Gem.GemSize + Padding)
                );
            }

            throw new Exception("Gem not found at this position");
        }

        private static Point? CoordToGemIndex(Vector2 coord)
        {
            try
            {
                return PositionToGemIndex(coord);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsValidSwap(Point origin, Point destiny)
        {
            var xDiff = Math.Abs(origin.X - destiny.X);
            var yDiff = Math.Abs(origin.Y - destiny.Y);

            return ((xDiff == 1 && yDiff == 0) || (yDiff == 1 && xDiff == 0));
        }

        private void HandleClick(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                firstClick = true;
            }

            if (mouseState.LeftButton == ButtonState.Released && firstClick)
            {
                firstClick = false;
                var gemIndex = CoordToGemIndex(mouseState.Position.ToVector2());

                // Current gem not found or its not a valid swap
                if (
                    gemIndex == null
                    || (firstGem.HasValue && !IsValidSwap(gemIndex.Value, firstGem.Value))
                )
                {
                    if (firstGem.HasValue)
                    {
                        state[firstGem.Value.X][firstGem.Value.Y].SetSelected(false);
                        firstGem = null;
                    }

                    return;
                }

                if (!firstGem.HasValue)
                {
                    state[gemIndex.Value.X][gemIndex.Value.Y].SetSelected(true);
                    firstGem = gemIndex.Value;
                }
                else // Is a valid swap, and the first gem is already selected
                {
                    state[firstGem.Value.X][firstGem.Value.Y].SetSelected(false); // Clean selected status
                    Vector2 destinyPosition = state[gemIndex.Value.X][gemIndex.Value.Y].position;
                    Vector2 originPosition = state[firstGem.Value.X][firstGem.Value.Y].position;

                    animationManager.EnqueueAnimation(
                        new AnimationItem[]
                        {
                            new AnimationItem(
                                state[firstGem.Value.X][firstGem.Value.Y],
                                AnimationType.Translation,
                                destinyPosition
                            ),
                            new AnimationItem(
                                state[gemIndex.Value.X][gemIndex.Value.Y],
                                AnimationType.Translation,
                                originPosition
                            ),
                        }
                    );

                    firstGem = null;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            var animationBatch = animationManager.GetAnimationItems();

            if (animationBatch != null)
            {
                foreach (var animation in animationBatch)
                {
                    if (animation.Type == AnimationType.Translation)
                    {
                        animation.Gem.MoveTo(animation.Destiny);
                    }
                    else if (animation.Type == AnimationType.Matching)
                    {
                        animation.Gem.Match(
                            () => RemoveGemAt(PositionToGemIndex(animation.Gem.position))
                        );
                    }
                }
            }

            foreach (var col in state)
            {
                foreach (var row in col)
                {
                    row.Update(gameTime);
                }
            }

            MouseState mState = Mouse.GetState();
            HandleClick(Mouse.GetState());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var col in state)
            {
                foreach (var row in col)
                {
                    row.Draw(spriteBatch);
                }
            }
        }
    }
}
