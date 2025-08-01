using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class BeatMapEntry
    {
        public string Title { get; private set; }
        public string Artist { get; private set; }

        private Texture2D _texture;

        private const int ENTRY_WIDTH = 1450;
        private const int ENTRY_HEIGHT = 180;

        public float CurrentX;
        public float TargetX;

        public BeatMapEntry(Texture2D texture, string title, string artist, float initialX)
        {
            _texture = texture;
            Title = title;
            Artist = artist;
            CurrentX = initialX;
            TargetX = initialX;
        }

        public void Update(int index, float scrollPosition, bool isHovered, bool isSelected)
        {
            float offset = index - scrollPosition;

            float baseX = 1284f;
            float driftX = Math.Min(Math.Abs(offset), 5f) * 10f;
            TargetX = baseX + driftX;

            if (isSelected)
                TargetX = 1061f;
            else if (isHovered)
                TargetX = 1090f;

            CurrentX += (TargetX - CurrentX) * 0.2f;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, float y, float scale, float opacity)
        {
            int scaledWidth = (int)(ENTRY_WIDTH * scale);
            int scaledHeight = (int)(ENTRY_HEIGHT * scale);

            int drawX = (int)CurrentX + (ENTRY_WIDTH - scaledWidth) / 2;
            int drawY = (int)y + (ENTRY_HEIGHT - scaledHeight) / 2;

            spriteBatch.Draw(
                _texture,
                new Rectangle(drawX, drawY, scaledWidth, scaledHeight),
                Color.White * opacity
            );

            float textX = CurrentX + 500f;
            Vector2 titlePos = new Vector2(textX, drawY + 20f);
            Vector2 artistPos = new Vector2(textX, drawY + 20f + font.LineSpacing + 5f);

            spriteBatch.DrawString(
                font,
                Title,
                titlePos,
                Color.White * opacity,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );

            spriteBatch.DrawString(
                font,
                Artist,
                artistPos,
                Color.White * opacity,
                0f,
                Vector2.Zero,
                scale * 0.8f,
                SpriteEffects.None,
                0f
            );
        }
    }
}
