using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public abstract class Enemy
    {
        protected Vector2 _position;
        protected float _speed;
        protected int _row; // 0 = lower, 1 = upper
        protected Texture2D _texture;
        protected bool _isAlive = true;

        public Enemy(Texture2D texture, int row, float startX, float speed)
        {
            _texture = texture;
            _row = row;
            _speed = speed;
            float y = (row == 0) ? 946f : 589f;
            _position = new Vector2(startX, y);
        }

        public virtual void Update(GameTime gameTime)
        {
            _position.X -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_isAlive)
                spriteBatch.Draw(_texture, _position, Color.White);
        }


        public abstract bool CanBeHit(Vector2 hitCirclePosition, float radius);
        public abstract void OnHit();

        public bool IsAlive => _isAlive;
        public bool ContributesToAccuracy { get; set; } = false;

        public Vector2 Position => _position;
        public int Row => _row;
    }
}
