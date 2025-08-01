using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HajimariNoSignal
{
    public class Character
    {
        private List<Texture2D> _idleFrames;
        private List<List<Texture2D>> _attackAnimations;
        private List<List<Texture2D>> _doubleAttacks;
        private List<Texture2D> _hurtFrames;

        private int _idleIndex = 0;
        private int _idleDelay = 1;
        private int _idleCounter = 0;

        private int _attackIndex = 0;
        private int _attackDelay = 1;
        private int _attackCounter = 0;

        private bool _isAttacking = false;
        private bool _isDoubleAttacking = false;
        private int _attackStage = 0;
        private int _doubleStage = 0;

        private bool _isHurt = false;
        private float _hurtTimer = 0f;
        private float _hurtDuration = 0.5f;

        private readonly int _x = 140;
        private float _currentY;
        private float _targetY;

        private const float ROW_0_Y = 846f;
        private const float ROW_1_Y = 489f;
        private const float MOVE_SPEED = 75f;

        //private int _hp = 5;
        private int _hurtIndex = 0;
        private float _hurtFrameTime = 0f;
        private float _hurtFrameDelay = 0.00f; // Tune as needed for frame rate

        public Character(List<Texture2D> idleFrames, List<List<Texture2D>> attackAnimations, List<List<Texture2D>> doubleAttacks, List<Texture2D> hurtFrames)
        {
            _idleFrames = idleFrames;
            _attackAnimations = attackAnimations;
            _doubleAttacks = doubleAttacks;
            _hurtFrames = hurtFrames;

            _currentY = ROW_0_Y;
            _targetY = ROW_0_Y;
        }

        public void Update()
        {
            if (_isHurt)
            {
                _hurtTimer += 1f / 60f; // or use delta time
                _hurtFrameTime += 1f / 60f;

                if (_hurtFrameTime >= _hurtFrameDelay && _hurtIndex < _hurtFrames.Count - 1)
                {
                    _hurtIndex++;
                    _hurtFrameTime = 0f;
                }

                if (_hurtTimer >= _hurtDuration)
                {
                    _isHurt = false;
                    _hurtTimer = 0f;
                    _hurtIndex = 0;
                    _hurtFrameTime = 0f;
                }
            }


            KeyboardState kb = Keyboard.GetState();

            if (!_isAttacking && !_isDoubleAttacking && !_isHurt)
            {
                if (kb.IsKeyDown(Keys.Space))
                {
                    StartDoubleAttack();
                }
                else if (kb.IsKeyDown(Keys.K))
                {
                    SetRow(0);
                    StartAttack();
                }
                else if (kb.IsKeyDown(Keys.S))
                {
                    SetRow(1);
                    StartAttack();
                }
            }

            UpdateY();

            if (_isDoubleAttacking)
                UpdateDoubleAttack();
            else if (_isAttacking)
                UpdateAttack();
            else
                UpdateIdle();
        }

        private void SetRow(int row)
        {
            _targetY = row == 0 ? ROW_0_Y : ROW_1_Y;
        }

        private void UpdateY()
        {
            if (_currentY < _targetY)
            {
                _currentY += MOVE_SPEED;
                if (_currentY > _targetY)
                    _currentY = _targetY;
            }
            else if (_currentY > _targetY)
            {
                _currentY -= MOVE_SPEED;
                if (_currentY < _targetY)
                    _currentY = _targetY;
            }
        }

        private void StartAttack()
        {
            _isAttacking = true;
            _isDoubleAttacking = false;
            _attackIndex = 0;
            _attackCounter = 0;
        }

        private void StartDoubleAttack()
        {
            _isDoubleAttacking = true;
            _isAttacking = false;
            _attackIndex = 0;
            _attackCounter = 0;
            _targetY = ((ROW_0_Y + ROW_1_Y) / 2f - 32f) - 200;
        }

        private void UpdateIdle()
        {
            _idleCounter++;
            if (_idleCounter >= _idleDelay)
            {
                _idleIndex = (_idleIndex + 1) % _idleFrames.Count;
                _idleCounter = 0;
            }
        }

        private void UpdateAttack()
        {
            var frames = _attackAnimations[_attackStage];
            _attackCounter++;

            if (_attackCounter >= _attackDelay)
            {
                _attackIndex++;
                _attackCounter = 0;

                if (_attackIndex >= frames.Count)
                {
                    _isAttacking = false;
                    _attackIndex = 0;
                    _attackStage = (_attackStage + 1) % _attackAnimations.Count;
                }
            }
        }

        private void UpdateDoubleAttack()
        {
            var frames = _doubleAttacks[_doubleStage];
            _attackCounter++;

            if (_attackCounter >= _attackDelay)
            {
                _attackIndex++;
                _attackCounter = 0;

                if (_attackIndex >= frames.Count)
                {
                    _isDoubleAttacking = false;
                    _attackIndex = 0;
                    _doubleStage = (_doubleStage + 1) % _doubleAttacks.Count;
                    _targetY = ROW_1_Y;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D frame;

            if (_isHurt)
                frame = _hurtFrames[Math.Min(_hurtIndex, _hurtFrames.Count - 1)];
            else if (_isDoubleAttacking)
                frame = _doubleAttacks[_doubleStage][_attackIndex];
            else if (_isAttacking)
                frame = _attackAnimations[_attackStage][_attackIndex];
            else
                frame = _idleFrames[_idleIndex];

            spriteBatch.Draw(frame, new Vector2(_x, _currentY), Color.White);

            //System.Diagnostics.Debug.WriteLine($"Character Position: {_x}, {_currentY}, HP: {_hp}");
        }

        public void PlayAttack(int row)
        {
            SetRow(row);
            StartAttack();
        }

        public void PlayDoubleAttack()
        {
            StartDoubleAttack();
        }

        public void TakeDamage()
        {
            if (_isHurt) return;

            //_hp--;
            _isHurt = true;
            _hurtTimer = 0f;

            //System.Diagnostics.Debug.WriteLine($"Character took damage! HP now: {_hp}");
        }
        public void Reset()
        {
            _idleIndex = 0;
            _idleCounter = 0;

            _attackIndex = 0;
            _attackCounter = 0;
            _attackStage = 0;

            _doubleStage = 0;
            _isAttacking = false;
            _isDoubleAttacking = false;

            _isHurt = false;
            _hurtTimer = 0f;
            _hurtIndex = 0;
            _hurtFrameTime = 0f;

            _currentY = 846f; // default to row 0
            _targetY = 846f;
        }

        public Rectangle GetCollisionBox()
        {
            return new Rectangle(_x, (int)_currentY, 150, 150); // Adjust size as needed
        }

        public bool IsInvincible()
        {
            return _isHurt;
        }

    }
}
