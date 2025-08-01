using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HajimariNoSignal
{
    public class GameOverMenu
    {
        private Texture2D _background;
        private Rectangle _backgroundRect;

        private Texture2D _uiFrame;
        private Rectangle _frameRect;


        private Texture2D _backButton;
        private Rectangle _backButtonRect;

        private MouseState _previousMouse;
        public GameOverMenu(Texture2D background, Texture2D uiFrame, Texture2D backButton)
        {
            _background = background;
            _uiFrame = uiFrame;
            _backButton = backButton;

            _backgroundRect = ImageFitter.GetFillingRectangle(_background, 2284, 1536);
            _frameRect = _backgroundRect;

            // Centered 400x150 button
            int buttonWidth = 370;
            int buttonHeight = 170;
            _backButtonRect = new Rectangle(
                (2284 - buttonWidth) / 2,
                (1536 - buttonHeight) / 2,
                buttonWidth, buttonHeight
            );
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

                System.Diagnostics.Debug.WriteLine("Back to BeatMap Menu.");
            }

            _previousMouse = mouse;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, _backgroundRect, Color.White);
            spriteBatch.Draw(_uiFrame, _frameRect, Color.White);
            spriteBatch.Draw(_backButton, _backButtonRect, Color.White);
        }

    }
}
