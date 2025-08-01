using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class FakeEnemy : Enemy
    {
        private Character _character;

        public FakeEnemy(Texture2D texture, int row, float startX, float speed, Character character)
            : base(texture, row, startX, speed)
        {
            _character = character;
            ContributesToAccuracy = false;
        }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            // Deal damage to the player instead of dying
            _character.TakeDamage();
            System.Diagnostics.Debug.WriteLine("FakeEnemy hit! Player damaged.");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isAlive)
                spriteBatch.Draw(_texture, _position, Color.Red); // or tint it red to warn the player
        }
    }
}
