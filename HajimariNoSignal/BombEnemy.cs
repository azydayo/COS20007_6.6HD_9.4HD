using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class BombEnemy : Enemy
    {
        private BossEnemy _bossTarget;
        private bool _returning = false;
        private Vector2 _velocityToBoss;


        public BombEnemy(Texture2D texture, int row, float startX, float speed, BossEnemy boss)
            : base(texture, row, startX, speed)
        {
            _bossTarget = boss;
            ContributesToAccuracy = true;
        }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            if (_bossTarget != null)
            {
                // Begin flying toward boss
                Vector2 direction = _bossTarget.Position - _position;
                direction.Normalize();
                _velocityToBoss = direction * 900f; // Speed of return
                _returning = true;

                System.Diagnostics.Debug.WriteLine("BombEnemy hit! Returning to boss.");
            }
        }
        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_returning)
            {
                _position += _velocityToBoss * delta;

                if (Vector2.Distance(_position, _bossTarget.Position) < 50f)
                {
                    _bossTarget?.TriggerDamageFlash();
                    _isAlive = false;
                }
            }
            else
            {
                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isAlive)
                spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}
