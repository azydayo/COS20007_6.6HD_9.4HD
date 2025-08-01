using HajimariNoSignal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class MainMenu
{
    private Texture2D _titleImage;

    private AnimatedBackground _background;
    private Texture2D _startButton;
    private Vector2 _buttonPosition;
    private Rectangle _buttonBounds;

    private Game1 _game;

    public MainMenu(Game1 game, List<Texture2D> backgroundFrames, Texture2D startButton)
    {
        _game = game;
        _background = new AnimatedBackground(backgroundFrames, frameDelay: 5);
        _startButton = startButton;

        _buttonPosition = new Vector2(
            (2284 - _startButton.Width) / 2,
            (1536 - _startButton.Height) / 2 + 300
        );

        _buttonBounds = new Rectangle(
            (int)_buttonPosition.X,
            (int)_buttonPosition.Y,
            _startButton.Width,
            _startButton.Height
        );

        _titleImage = _game.Content.Load<Texture2D>("skin/yugen");

    }

    public void Update(GameTime gameTime)
    {
        _background.Update();

        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && _buttonBounds.Contains(mouse.Position))
        {
            _game.AudioManager.PlaySFX("click");
            _game.AudioManager.StopMusic(); // Optional
            Game1.Instance._transitionManager.Start(GameState.BeatMapMenu);
        }
    }
    private Texture2D CreateOverlayTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White }); // pure white pixel
        return texture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Layer 1
        _background.Draw(spriteBatch);

        //Layer 2
        Texture2D overlay = CreateOverlayTexture(_game.GraphicsDevice);
        spriteBatch.Draw(overlay, new Rectangle(0, 0, 2284, 1536), Color.Black * 0.5f); // 50% opacity

        // Layer 3, Game title
        Vector2 titlePosition = new Vector2(
            (2284 - _titleImage.Width) / 2,
            (1536 - _titleImage.Height) / 2
        );
        spriteBatch.Draw(_titleImage, titlePosition, Color.White);

        //Layer 3
        spriteBatch.Draw(_startButton, _buttonPosition, Color.White);
    }
    public AnimatedBackground GetBackground()
    {
        return _background;
    }

}
