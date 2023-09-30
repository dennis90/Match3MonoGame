using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Match3Mono
{
    public enum AnimationType
    {
        None = 0,
        Translation,
        Matching,
    }

    public class AnimationItem
    {
        private readonly Gem gem = null;
        private readonly Vector2 destiny = Vector2.Zero;
        private readonly AnimationType type = AnimationType.None;

        public AnimationItem(Gem gem, AnimationType type, Vector2 destiny)
        {
            this.gem = gem;
            this.type = type;
            this.destiny = destiny;
        }

        public AnimationItem(Gem gem, AnimationType type)
        {
            this.gem = gem;
            this.type = type;
            this.destiny = Vector2.Zero;
        }

        public Gem Gem => this.gem;
        public AnimationType Type => this.type;
        public Vector2 Destiny => this.destiny;
    }

    public class AnimationManager
    {
        private int animationCount = 0;
        private readonly List<AnimationItem[]> enqueuedItems = new();

        public void EnqueueAnimation(AnimationItem[] items)
        {
            enqueuedItems.Add(items);
        }

        public AnimationItem[] GetAnimationItems()
        {
            if (animationCount != 0 || enqueuedItems.Count == 0)
            {
                return null;
            }

            var first = enqueuedItems[0];
            enqueuedItems.RemoveAt(0);
            this.animationCount += first.Length;
            return first;
        }

        public void OnAnimationCompleted()
        {
            this.animationCount--;
        }
    }
}
