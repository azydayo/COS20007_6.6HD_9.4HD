using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class CustomCursor
{
    private Texture2D _cursorTexture;

    public CustomCursor(Texture2D cursorTexture)
    {
        _cursorTexture = cursorTexture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        MouseState mouse = Mouse.GetState();
        Vector2 position = new Vector2(mouse.X, mouse.Y);
        Vector2 origin = new Vector2(_cursorTexture.Width / 2f, _cursorTexture.Height / 2f);

        spriteBatch.Draw(_cursorTexture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 1f);
    }
}
