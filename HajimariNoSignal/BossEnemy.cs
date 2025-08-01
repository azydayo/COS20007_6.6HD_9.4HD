using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HajimariNoSignal
{
    public class BossEnemy : Enemy
    {
        public enum BossState { Appear, Idle, AirAttack, LowAttack, Exit }

        private BossState _state = BossState.Appear;
        private Dictionary<BossState, List<Texture2D>> _animations;
        private int _animIndex = 0;
        private int _frameCounter = 0;
        private int _frameDelay = 6;

        private float _spawnCooldown = 2.5f;
        private float _spawnTimer = 0f;

        private float _lifeTime = 100f;
        private float _lifeTimer = 0f;

        private float _damageFlashTimer = 0f;
        private const float FLASH_DURATION = 0.3f;

        private Color _tintColor = Color.White;


        private BeatMap _beatMap;

        public BossEnemy(Dictionary<BossState, List<Texture2D>> animations, BeatMap beatMap)
            : base(animations[BossState.Appear][0], -1, 1700f, 0f)
        {
            _animations = animations;
            _beatMap = beatMap;
            _position = new Vector2(1500f, 400f);
            ContributesToAccuracy = false;
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _spawnTimer += delta;
            _lifeTimer += delta;

            _frameCounter++;
            if (_frameCounter >= _frameDelay)
            {
                _frameCounter = 0;
                _animIndex++;

                var currentAnim = _animations[_state];

                if (_state == BossState.Appear && _animIndex >= currentAnim.Count)
                {
                    _state = BossState.Idle;
                    _animIndex = 0;
                }
                else if (_state == BossState.Exit && _animIndex >= currentAnim.Count)
                {
                    _isAlive = false;
                    _animIndex = currentAnim.Count - 1;
                    return;
                }
                else if (_animIndex >= currentAnim.Count)
                {
                    _animIndex = 0;
                }
            }

            if (_state == BossState.Idle || _state == BossState.AirAttack || _state == BossState.LowAttack)
            {
                if (_spawnTimer >= _spawnCooldown)
                {
                    _spawnTimer = 0f;

                    if (_state == BossState.Idle)
                        _state = BossState.AirAttack;
                    else if (_state == BossState.AirAttack)
                        _state = BossState.LowAttack;
                    else
                        _state = BossState.Idle;

                    if (_state == BossState.AirAttack)
                        _beatMap.SpawnBossEnemy(1, _position);
                    else if (_state == BossState.LowAttack)
                        _beatMap.SpawnBossEnemy(0, _position);
                }
            }

            if (_lifeTimer >= _lifeTime && _state != BossState.Exit)
            {
                _state = BossState.Exit;
                _animIndex = 0;
            }

            if (_damageFlashTimer > 0)
            {
                _damageFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_damageFlashTimer < 0)
                {
                    _damageFlashTimer = 0;
                    _tintColor = Color.White;
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!_isAlive) return;

            if (!_animations.ContainsKey(_state) || _animations[_state].Count == 0)
                return;

            if (_animIndex >= _animations[_state].Count)
                _animIndex = _animations[_state].Count - 1;

            Texture2D current = _animations[_state][_animIndex];
            spriteBatch.Draw(current, _position, _tintColor);

        }
        public void TriggerDamageFlash()
        {
            _damageFlashTimer = FLASH_DURATION;
            _tintColor = Color.Red;
            TakeBombDamage();
        }

        public void TakeBombDamage()
        {
            _lifeTimer += 20f; // speeds up exit
            System.Diagnostics.Debug.WriteLine("Boss took damage from bomb!");
        }


        public override bool CanBeHit(Vector2 hitCirclePosition, float radius)
        {
            return false;
        }

        public override void OnHit()
        {
        }
    }
}
