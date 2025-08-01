using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class FlashEnemy : Enemy
    {
        private BeatMap _beatMap;

        public FlashEnemy(Texture2D texture, int row, float startX, float speed, BeatMap beatMap)
            : base(texture, row, startX, speed)
        {
            _beatMap = beatMap;
            ContributesToAccuracy = true;
        }

        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return _isAlive && Vector2.Distance(_position, hitCirclePosition) <= radius;
        }

        public override void OnHit()
        {
            _isAlive = false;
            _beatMap.TriggerScreenFlash(2f); // 2 seconds flash
            System.Diagnostics.Debug.WriteLine("FlashEnemy hit! Triggering screen flash.");
        }
    }
}
