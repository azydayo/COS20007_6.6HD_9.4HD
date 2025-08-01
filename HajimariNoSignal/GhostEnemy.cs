using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class GhostEnemy : Enemy
    {
        public GhostEnemy(Texture2D texture, int row, float startX, float speed)
            : base(texture, row, startX, speed)
        {
        }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            _isAlive = false;
            System.Diagnostics.Debug.WriteLine("GhostEnemy hit!");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_isAlive) return;

            float distanceToHitLine = Math.Abs(_position.X - 625f); // match HIT_LINE_X
            float alpha = MathHelper.Clamp(distanceToHitLine / 1000f, 0.2f, 1f); // farther = more visible

            spriteBatch.Draw(_texture, _position, Color.White * alpha);
        }
    }
}
