using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class BossSpawnEnemy : Enemy
    {
        public BossSpawnEnemy(Texture2D texture, int row, Vector2 bossPosition)
            : base(texture, row, bossPosition.X - 100f, 600f) // Spawn just in front of boss
        {
            ContributesToAccuracy = false;
        }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            _isAlive = false;
            System.Diagnostics.Debug.WriteLine("Boss-Spawned Enemy Hit!");
        }
    }
}
