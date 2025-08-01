using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HajimariNoSignal
{
    public class FinalMenu
    {
        private Texture2D _background, _uiFrame, _backButton;
        private Rectangle _backgroundRect, _frameRect, _backButtonRect;
        private MouseState _previousMouse;
        private Dictionary<string, Texture2D> _rankTextures;


        private ResultData resultData;
        public FinalMenu(Texture2D background, Texture2D uiFrame, Texture2D backButton)
        {
            _background = background;
            _uiFrame = uiFrame;
            _backButton = backButton;


            _backgroundRect = ImageFitter.GetFillingRectangle(_background, 2284, 1536);
            _frameRect = ImageFitter.GetFillingRectangle(_background, 2284, 1536);

            int buttonWidth = 370;
            int buttonHeight = 170;
            _backButtonRect = new Rectangle(
                120, 50,
                buttonWidth, buttonHeight
            );

            _rankTextures = new Dictionary<string, Texture2D>
            {
                { "S", Game1.StaticContent.Load<Texture2D>("Ranks/S") },
                { "A", Game1.StaticContent.Load<Texture2D>("Ranks/A") },
                { "B", Game1.StaticContent.Load<Texture2D>("Ranks/B") },
                { "C", Game1.StaticContent.Load<Texture2D>("Ranks/C") },
                { "D", Game1.StaticContent.Load<Texture2D>("Ranks/D") },
                { "X", Game1.StaticContent.Load<Texture2D>("Ranks/X") }
            };

        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed &&
                _previousMouse.LeftButton == ButtonState.Released &&
                _backButtonRect.Contains(mouse.Position))
            {
                Game1.Instance._transitionManager.Start(GameState.BeatMapMenu);
                Game1.Instance.AudioManager.PlaySFX("back");
                System.Diagnostics.Debug.WriteLine("Back to BeatMap Menu from Final.");
            }

            _previousMouse = mouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, _backgroundRect, Color.White);
            spriteBatch.Draw(_uiFrame, _frameRect, Color.White);
            spriteBatch.Draw(_backButton, _backButtonRect, Color.White);

            ResultData result = Game1.Instance.ResultData;

            string scoreText = $"Score: {result.Score}";
            string hitsText = $"Hits: {result.HitCount}  Misses: {result.MissCount}";

            SpriteFont font = Game1.Instance._font;
            Vector2 basePos = new Vector2(100, 800);

            spriteBatch.DrawString(font, scoreText, basePos, Color.Yellow);
            spriteBatch.DrawString(font, hitsText, basePos + new Vector2(0, 200), Color.Yellow);
            string rank = result.GetRank();
            if (_rankTextures.TryGetValue(rank, out Texture2D rankTex))
            {
                Vector2 rankPosition = new Vector2(1750, 80); //1800 100 org
                spriteBatch.Draw(rankTex, rankPosition, Color.White);
            }
        }
    }
}
