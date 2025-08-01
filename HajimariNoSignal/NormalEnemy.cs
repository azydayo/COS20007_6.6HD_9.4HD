using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class NormalEnemy : Enemy
    {
        public NormalEnemy(Texture2D texture, int row, float startX, float speed)
            : base(texture, row, startX, speed) { ContributesToAccuracy = true; }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            _isAlive = false;
            System.Diagnostics.Debug.WriteLine("Normal Enemy Hit!");
        }
    }
}