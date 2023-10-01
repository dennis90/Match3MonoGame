using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Match3Mono
{
    public class GameBoard
    {
        const int BoardWidth = 8;
        const int BoardHeight = 8;
        const int Padding = 10;
        const int MarginX = 20;
        const int MarginY = 50;

        private Gem[,] BoardState;
        private readonly AnimationManager animationManager = new();
        private readonly Random rand = new();
        private readonly Texture2D[] textureList;
        private readonly SpriteFont font;
        public int PlayerScore = 0;

        public bool DebugInfoEnabled = false;

        private bool firstClick = false;
        Point? firstGem = null;

        private string _playedAnimation = "";

        public GameBoard(Texture2D[] _textureList, SpriteFont _font)
        {
            this.textureList = _textureList;
            this.font = _font;

            BoardState = new Gem[BoardWidth, BoardHeight];

            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    int textureIndex = rand.Next(0, textureList.Length);
                    BoardState[x, y] = new Gem(
                        textureIndex,
                        textureList[textureIndex],
                        new Vector2(
                            MarginX + (x * (Gem.GemSize + Padding)),
                            MarginY + (y * (Gem.GemSize + Padding))
                        ),
                        animationManager.OnAnimationCompleted,
                        font
                    );
                }
            }
        }

        private Gem[,] GetFilledBoard(Gem[,] board, bool animate = false)
        {
            List<AnimationItem> fillAnimations = new();
            var nextBoard = CloneBoard(board);

            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    var gemSprite = GemSpriteAt(nextBoard, new Point(x, y));

                    if (gemSprite >= 0)
                    {
                        continue;
                    }

                    int textureIndex = rand.Next(0, textureList.Length);

                    int xPos = x * (Gem.GemSize + Padding) + MarginX;
                    int yPos = y * (Gem.GemSize + Padding) + MarginY;

                    var nextGem = new Gem(
                        textureIndex,
                        textureList[textureIndex],
                        new Vector2(xPos, animate ? Gem.GemSize * -1 : yPos),
                        animationManager.OnAnimationCompleted,
                        font
                    );

                    nextBoard[x, y] = nextGem;

                    if (animate)
                    {
                        if (DebugInfoEnabled)
                        {
                            Debug.WriteLine("+++ GetFilledBoard");
                        }

                        fillAnimations.Add(
                            new AnimationItem(
                                nextGem,
                                AnimationType.Translation,
                                new Vector2(xPos, yPos)
                            )
                        );
                    }
                }
            }

            if (fillAnimations.Count > 0)
            {
                animationManager.EnqueueAnimation(fillAnimations.ToArray());
            }

            return nextBoard;
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

        private static Gem[,] CloneBoard(Gem[,] origin)
        {
            Gem[,] result = (Gem[,])origin.Clone();
            return result;
        }

        private static Gem GemAt(Gem[,] board, Point position)
        {
            if (
                position.X >= 0
                && position.X < BoardWidth
                && position.Y >= 0
                && position.Y < BoardHeight
            )
            {
                return board[position.X, position.Y];
            }

            return null;
        }

        private static int GemSpriteAt(Gem[,] board, Point position)
        {
            var gem = GemAt(board, position);

            if (gem != null)
            {
                return gem.SpriteIndex;
            }

            return -1;
        }

        private static List<Point[]> GetMatches(Gem[,] board)
        {
            List<Point[]> GemsToRemove = new();

            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    var currentGem = GemSpriteAt(board, new Point(x, y));

                    if (
                        currentGem != -1
                        && GemSpriteAt(board, new Point(x + 1, y)) == currentGem
                        && GemSpriteAt(board, new Point(x + 2, y)) == currentGem
                    )
                    {
                        int offset = 0;
                        List<Point> removeSet = new();

                        while (GemSpriteAt(board, new Point(x + offset, y)) == currentGem)
                        {
                            removeSet.Add(new Point(x + offset, y));
                            offset++;
                        }

                        GemsToRemove.Add(removeSet.ToArray());
                    }

                    if (
                        currentGem != -1
                        && GemSpriteAt(board, new Point(x, y + 1)) == currentGem
                        && GemSpriteAt(board, new Point(x, y + 2)) == currentGem
                    )
                    {
                        int offset = 0;
                        List<Point> removeSet = new();

                        while (GemSpriteAt(board, new Point(x, y + offset)) == currentGem)
                        {
                            removeSet.Add(new Point(x, y + offset));
                            offset++;
                        }

                        GemsToRemove.Add(removeSet.ToArray());
                    }
                }
            }

            return GemsToRemove;
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
                        BoardState[firstGem.Value.X, firstGem.Value.Y].SetSelected(false);
                        firstGem = null;
                    }

                    return;
                }

                if (!firstGem.HasValue)
                {
                    BoardState[gemIndex.Value.X, gemIndex.Value.Y].SetSelected(true);
                    firstGem = gemIndex.Value;
                }
                else // Is a valid swap, and the first gem is already selected
                {
                    BoardState[firstGem.Value.X, firstGem.Value.Y].SetSelected(false); // Clean selected status
                    Vector2 destinyPosition = new Vector2(
                        BoardState[gemIndex.Value.X, gemIndex.Value.Y].position.X,
                        BoardState[gemIndex.Value.X, gemIndex.Value.Y].position.Y
                    );
                    Vector2 originPosition = new Vector2(
                        BoardState[firstGem.Value.X, firstGem.Value.Y].position.X,
                        BoardState[firstGem.Value.X, firstGem.Value.Y].position.Y
                    );
                    if (DebugInfoEnabled)
                    {
                        Debug.WriteLine("+++ Swap");
                    }
                    animationManager.EnqueueAnimation(
                        new AnimationItem[]
                        {
                            new AnimationItem(
                                BoardState[firstGem.Value.X, firstGem.Value.Y],
                                AnimationType.Translation,
                                destinyPosition
                            ),
                            new AnimationItem(
                                BoardState[gemIndex.Value.X, gemIndex.Value.Y],
                                AnimationType.Translation,
                                originPosition
                            ),
                        }
                    );

                    var tempBoard = CloneBoard(BoardState);

                    (
                        tempBoard[gemIndex.Value.X, gemIndex.Value.Y],
                        tempBoard[firstGem.Value.X, firstGem.Value.Y]
                    ) = (
                        tempBoard[firstGem.Value.X, firstGem.Value.Y],
                        tempBoard[gemIndex.Value.X, gemIndex.Value.Y]
                    );

                    var FoundMatches = GetMatches(tempBoard);

                    if (FoundMatches.Count == 0)
                    {
                        if (DebugInfoEnabled)
                        {
                            Debug.WriteLine("+++ Revert Swap");
                        }
                        animationManager.EnqueueAnimation(
                            new AnimationItem[]
                            {
                                new AnimationItem(
                                    BoardState[firstGem.Value.X, firstGem.Value.Y],
                                    AnimationType.Translation,
                                    originPosition
                                ),
                                new AnimationItem(
                                    BoardState[gemIndex.Value.X, gemIndex.Value.Y],
                                    AnimationType.Translation,
                                    destinyPosition
                                ),
                            }
                        );
                    }
                    else
                    {
                        (
                            BoardState[gemIndex.Value.X, gemIndex.Value.Y],
                            BoardState[firstGem.Value.X, firstGem.Value.Y]
                        ) = (
                            BoardState[firstGem.Value.X, firstGem.Value.Y],
                            BoardState[gemIndex.Value.X, gemIndex.Value.Y]
                        );
                    }

                    firstGem = null;
                }
            }
        }

        private void BubbleEmpty(Gem[,] board)
        {
            List<AnimationItem> animations = new();
            int totalMoved = 0;

            for (int x = 0; x < BoardWidth; x++)
            {
                List<Gem> gemsInColumn = new();
                for (int y = 0; y < BoardHeight; y++)
                {
                    if (GemSpriteAt(board, new Point(x, y)) != -1)
                    {
                        gemsInColumn.Add(board[x, y]);
                    }
                }

                if (gemsInColumn.Count == BoardHeight)
                {
                    continue;
                }

                totalMoved += BoardHeight - gemsInColumn.Count;

                for (int y = BoardHeight - 1; y >= 0; y--)
                {
                    if (gemsInColumn.Count > 0)
                    {
                        var lastGem = gemsInColumn[gemsInColumn.Count - 1];
                        gemsInColumn.RemoveAt(gemsInColumn.Count - 1);
                        int nextPosition = MarginY + (y * (Gem.GemSize + Padding));

                        board[x, y] = lastGem;

                        if (Math.Abs(lastGem.position.Y - nextPosition) > 5)
                        {
                            if (lastGem.GetDestiny().Y != nextPosition)
                            {
                                animations.Add(
                                    new AnimationItem(
                                        lastGem,
                                        AnimationType.Translation,
                                        new Vector2(lastGem.position.X, nextPosition)
                                    )
                                );
                            }
                        }
                        else
                        {
                            lastGem.position.Y = nextPosition;
                        }
                    }
                    else
                    {
                        board[x, y] = null;
                    }
                }
            }

            if (animations.Count > 0)
            {
                if (DebugInfoEnabled)
                {
                    Debug.WriteLine("+++ Bubble");
                }
                animationManager.EnqueueAnimation(animations.ToArray());
            }

            if (totalMoved > 0)
            {
                var nextBoard = GetFilledBoard(board, true);
                BoardState = nextBoard;
            }
        }

        public void Update(GameTime gameTime)
        {
            var animationBatch = animationManager.GetAnimationItems();

            if (animationBatch != null)
            {
                foreach (var animation in animationBatch)
                {
                    _playedAnimation = animation.Type.ToString();

                    if (animation.Type == AnimationType.Translation)
                    {
                        animation.Gem.MoveTo(animation.Destiny);
                    }
                    else if (animation.Type == AnimationType.Matching)
                    {
                        animation.Gem.Match();
                    }
                }
            }

            var matches = GetMatches(BoardState);

            if (matches.Count > 0)
            {
                int scoreAdd = 0;

                foreach (var match in matches)
                {
                    List<AnimationItem> batch = new();

                    for (int i = 0; i < match.Length; i++)
                    {
                        if (DebugInfoEnabled)
                        {
                            Debug.WriteLine(
                                "Found Match: ("
                                    + match[i].X.ToString()
                                    + ", "
                                    + match[i].Y.ToString()
                                    + ")"
                            );
                        }
                        if (!BoardState[match[i].X, match[i].Y].IsQueuedForMatch)
                        {
                            scoreAdd += (10 + match.Length - 3) * 10;
                            BoardState[match[i].X, match[i].Y].QueueForMatch();

                            batch.Add(
                                new AnimationItem(
                                    BoardState[match[i].X, match[i].Y],
                                    AnimationType.Matching
                                )
                            );
                        }
                    }

                    if (batch.Count > 0)
                    {
                        if (DebugInfoEnabled)
                        {
                            Debug.WriteLine("+++ Matching");
                        }
                        animationManager.EnqueueAnimation(batch.ToArray());
                    }
                }

                PlayerScore += scoreAdd;
            }

            BubbleEmpty(BoardState);

            for (var x = 0; x < BoardWidth; x++)
            {
                for (var y = 0; y < BoardHeight; y++)
                {
                    if (BoardState[x, y] != null)
                    {
                        BoardState[x, y].DebugInfoEnabled = DebugInfoEnabled;
                    }
                    BoardState[x, y]?.Update(gameTime);
                }
            }

            MouseState mState = Mouse.GetState();
            HandleClick(Mouse.GetState());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (DebugInfoEnabled)
            {
                spriteBatch.DrawString(
                    font,
                    "Running "
                        + animationManager.RunningAnimations
                        + "\nEnqueued: "
                        + animationManager.PendingBatches
                        + "\nLast Processing: "
                        + _playedAnimation,
                    new Vector2(900, 50),
                    Color.DeepPink
                );
            }

            DrawScore(spriteBatch);
            for (var x = 0; x < BoardWidth; x++)
            {
                for (var y = 0; y < BoardHeight; y++)
                {
                    BoardState[x, y]?.Draw(spriteBatch);
                    if (DebugInfoEnabled)
                    {
                        if (BoardState[x, y] != null)
                        {
                            spriteBatch.DrawString(
                                font,
                                "(" + x + ", " + y + ")",
                                new Vector2(
                                    BoardState[x, y].position.X + 10,
                                    BoardState[x, y].position.Y + 20
                                ),
                                Color.DeepPink
                            );
                        }
                    }
                }
            }
        }

        private void DrawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                "Score: " + PlayerScore.ToString(),
                new Vector2(10, 10),
                Color.White
            );
        }
    }
}
