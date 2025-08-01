using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HajimariNoSignal
{
    public class AnimatedBackground
    {
        private readonly List<Texture2D> frames;
        private readonly int frameDelay;
        private int currentFrame;
        private int frameCounter;

        public AnimatedBackground(List<Texture2D> frames, int frameDelay = 5)
        {
            this.frames = frames;
            this.frameDelay = frameDelay;
            this.currentFrame = 0;
            this.frameCounter = 0;
        }

        public void Update()
        {
            frameCounter++;
            if (frameCounter >= frameDelay)
            {
                currentFrame = (currentFrame + 1) % frames.Count;
                frameCounter = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (frames.Count > 0)
            {
                Texture2D current = frames[currentFrame];
                Rectangle fitRect = ImageFitter.GetFillingRectangle(current, 2284, 1536);
                spriteBatch.Draw(current, fitRect, Color.White);
            }
        }
    }
}
