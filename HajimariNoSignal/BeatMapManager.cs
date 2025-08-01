using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace HajimariNoSignal
{
    public class BeatMap
    {
        private Texture2D _stageBackground;
        private float _scrollOffset;
        private float _scrollSpeed = 700f;

        private int _windowWidth = 2284;
        private int _windowHeight = 1536;

        private Rectangle _scrollRect;
        private int _scrollWidth;

        private Character _character;
        private Texture2D _hitCircleTexture;
        private Texture2D _heartTexture;
        private int _maxHearts = 5;
        private int _currentHearts = 5;

        private List<Texture2D> _normalEnemyTextures;
        private Texture2D _fakeEnemyTexture;
        private List<Texture2D> _bombEnemyTextures;

        private float _flashTimer = 0f;
        private Texture2D _whiteOverlay;

        private Dictionary<BossEnemy.BossState, List<Texture2D>> _bossAnimations;
        private Random _random = new Random();

        private List<Enemy> _enemies = new();
        private Queue<SpawnInstruction> _spawnQueue = new();
        private float _beatTimer = 0f;

        private Song _song;
        private float _offset;
        private bool _started = false;

        private const float HIT_LINE_X = 625f;
        private const float PERFECT_RANGE = 70f;

        private AudioManager _audioManager;

        private List<Texture2D> _bossSpawnTextures;

        private bool _isGameOver = false;

        private int _score = 0;
        private int _hitCount = 0;
        private int _missCount = 0;


        /*
        private readonly Dictionary<string, float> _difficultyModifiers = new()
        {
            { "Easy", 1.3f },
            { "Normal", 1.0f },
            { "Hard", 0.8f },
            { "Insane", 0.5f },
            { "Nightmare", 0.3f }
        };
        */
        public BeatMap(
            string beatmapPath,
            Texture2D stageBackground,
            Character character,
            Texture2D hitCircleTexture,
            List<Texture2D> normalEnemyTextures,
            AudioManager audioManager,
            Dictionary<BossEnemy.BossState, List<Texture2D>> bossAnimations,
            List<Texture2D> bossSpawnTextures,
            Texture2D fakeEnemyTexture,
            List<Texture2D> bombEnemyTextures,
            Texture2D heartTexture

        )
        {
            _stageBackground = stageBackground;
            _character = character;
            _hitCircleTexture = hitCircleTexture;
            _normalEnemyTextures = normalEnemyTextures;
            _audioManager = audioManager;
            _bossAnimations = bossAnimations;
            _bossSpawnTextures = bossSpawnTextures;
            _fakeEnemyTexture = fakeEnemyTexture;
            _bombEnemyTextures = bombEnemyTextures;


            _scrollRect = ImageFitter.GetFillingRectangle(_stageBackground, _windowWidth, _windowHeight);
            _scrollWidth = _scrollRect.Width;

            string json = File.ReadAllText(beatmapPath);
            BeatMapData data = JsonConvert.DeserializeObject<BeatMapData>(json);
            _offset = data.Offset;
            _song = Game1.StaticContent.Load<Song>(data.Audio);

            foreach (var note in data.Notes)
                _spawnQueue.Enqueue(note);

            _whiteOverlay = new Texture2D(_stageBackground.GraphicsDevice, 1, 1);

            _whiteOverlay.SetData(new[] { Color.White });

            _heartTexture = heartTexture;

        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _scrollOffset += _scrollSpeed * delta;
            if (_scrollOffset >= _scrollWidth)
                _scrollOffset -= _scrollWidth;

            _character.Update();
            HandleInput();
            _beatTimer += delta;

            if (!_started && _beatTimer >= _offset)
            {
                _audioManager.PlayMusic(_song, false);
                _started = true;
            }

            while (_spawnQueue.Count > 0 && _beatTimer >= _spawnQueue.Peek().Time + _offset)
            {
                var inst = _spawnQueue.Dequeue();
                Enemy enemy = null;

                if (inst.Type == "Normal")
                {
                    var tex = _normalEnemyTextures[_random.Next(_normalEnemyTextures.Count)];
                    enemy = new NormalEnemy(tex, inst.Row, 2400f, 600f);
                }
                else if (inst.Type == "Boss")
                {
                    var boss = new BossEnemy(_bossAnimations, this);
                    _enemies.Add(boss);
                }
                else if (inst.Type == "Flash")
                {
                    var tex = _normalEnemyTextures[_random.Next(_normalEnemyTextures.Count)];
                    enemy = new FlashEnemy(tex, inst.Row, 2400f, 600f, this);
                }
                else if (inst.Type == "Ghost")
                {
                    var tex = _normalEnemyTextures[_random.Next(_normalEnemyTextures.Count)];
                    enemy = new GhostEnemy(tex, inst.Row, 2400f, 600f);
                }
                else if (inst.Type == "Fake")
                {
                    enemy = new FakeEnemy(_fakeEnemyTexture, inst.Row, 2400f, 600f, _character);
                }
                else if (inst.Type == "Bomb")
                {
                    var tex = _bombEnemyTextures[_random.Next(_bombEnemyTextures.Count)];
                    enemy = new BombEnemy(tex, inst.Row, 2400f, 600f, GetCurrentBoss());
                }

                if (enemy != null)
                    _enemies.Add(enemy);
            }

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];
                enemy.Update(gameTime);

                if (!enemy.IsAlive)
                {
                    _enemies.RemoveAt(i);
                }
                else if (enemy.Position.X < -100)
                {
                    if (enemy is not FakeEnemy)
                        _missCount++;

                    _enemies.RemoveAt(i);
                }
            }

            // Flash timer update
            _flashTimer -= delta;
            if (_flashTimer < 0f)
                _flashTimer = 0f;

            // Collision check
            Rectangle charBox = _character.GetCollisionBox();

            foreach (var e in _enemies)
            {
                if (e.IsAlive && e.Row != -1)
                {
                    Rectangle enemyBox = new Rectangle((int)e.Position.X, (int)e.Position.Y, 100, 100);
                    if (charBox.Intersects(enemyBox))
                    {
                        DamagePlayer();
                        break;
                    }
                }
            }

            // ✅ Move this OUTSIDE the loop!
            if (_started && MediaPlayer.State == MediaState.Stopped && !_isGameOver)
            {
                TriggerFinal();
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            var rect1 = new Rectangle((int)(_scrollRect.X - _scrollOffset), _scrollRect.Y, _scrollRect.Width, _scrollRect.Height);
            var rect2 = new Rectangle(rect1.X + _scrollWidth, _scrollRect.Y, _scrollRect.Width, _scrollRect.Height);
            spriteBatch.Draw(_stageBackground, rect1, Color.White);
            spriteBatch.Draw(_stageBackground, rect2, Color.White);

            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch);

            _character.Draw(spriteBatch);

            Vector2 offset = new Vector2(250f, 200f);

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(
                pixel,
                new Rectangle((int)HIT_LINE_X, 0, 70, _windowHeight),
                Color.Yellow * 0.5f
            );

            if (_flashTimer > 0f)
            {
                float alpha = MathHelper.Clamp(_flashTimer / 2f, 0f, 1f); // fades over 2 seconds
                spriteBatch.Draw(_whiteOverlay, new Rectangle(0, 0, _windowWidth, _windowHeight), Color.White * alpha);
            }


            /*
            spriteBatch.Draw(
                _hitCircleTexture,
                new Rectangle((int)(HitCircle.Upper.X + offset.X - HitCircle.Radius), (int)(HitCircle.Upper.Y + offset.Y - HitCircle.Radius),
                              HitCircle.Radius * 2, HitCircle.Radius * 2),
                Color.White);

            spriteBatch.Draw(
                _hitCircleTexture,
                new Rectangle((int)(HitCircle.Lower.X + offset.X - HitCircle.Radius), (int)(HitCircle.Lower.Y + offset.Y - HitCircle.Radius),
                              HitCircle.Radius * 2, HitCircle.Radius * 2),
                Color.White);
            */
            //System.Diagnostics.Debug.WriteLine($"Upper Hit Circle: {HitCircle.Upper.X + 250}, {HitCircle.Upper.Y + 200}");
            //System.Diagnostics.Debug.WriteLine($"Lower Hit Circle: {HitCircle.Lower.X + 250}, {HitCircle.Lower.Y + 200}");

            int startX = 100;
            int startY = 1200;
            int heartWidth = 100;
            int heartHeight = 100;
            int spacing = 10;

            for (int i = 0; i < _currentHearts; i++)
            {
                spriteBatch.Draw(_heartTexture,
                    new Rectangle(startX + i * (heartWidth + spacing), startY, heartWidth, heartHeight),
                    Color.White);
            }

        }

        public void SpawnEnemy(int row)
        {
            var tex = _normalEnemyTextures[_random.Next(_normalEnemyTextures.Count)];
            var enemy = new NormalEnemy(tex, row, 2400f, 600f);
            _enemies.Add(enemy);
        }

        public void SpawnBossEnemy(int row, Vector2 bossPosition)
        {
            var tex = _bossSpawnTextures[_random.Next(_bossSpawnTextures.Count)];
            var enemy = new BossSpawnEnemy(tex, row, bossPosition);
            enemy.ContributesToAccuracy = false;
            _enemies.Add(enemy);
        }

        private BossEnemy GetCurrentBoss()
        {
            foreach (var e in _enemies)
            {
                if (e is BossEnemy boss && boss.IsAlive)
                    return boss;
            }
            return null;
        }

        public void DamagePlayer()
        {
            if (_character.IsInvincible() || _isGameOver) return;

            if (_currentHearts > 0)
                _currentHearts--;

            _character.TakeDamage();

            System.Diagnostics.Debug.WriteLine($"Player HP: {_currentHearts}/{_maxHearts}");

            if (_currentHearts <= 0)
            {
                TriggerGameOver();
            }
        }


        private void TriggerGameOver()
        {
            _isGameOver = true;
            _audioManager.StopMusic();
            _spawnQueue.Clear(); // stop spawning new enemies

            Game1.Instance._transitionManager.Start(GameState.GameOver);
            System.Diagnostics.Debug.WriteLine("===> GAME OVER triggered.");
        }

        private void TriggerFinal()
        {
            _audioManager.StopMusic();
            Game1.Instance._transitionManager.Start(GameState.Final);

            Game1.Instance.ResultData = new ResultData
            {
                Score = _score,
                HitCount = _hitCount,
                MissCount = _missCount,
            };


            System.Diagnostics.Debug.WriteLine("===> FINALIZATION triggered.");

            System.Diagnostics.Debug.WriteLine($"MediaPlayer State: {MediaPlayer.State}");
            System.Diagnostics.Debug.WriteLine($"Song Duration: {_song.Duration.TotalSeconds}");

        }

        public void TriggerScreenFlash(float duration)
        {
            _flashTimer = duration;
        }

        public void HandleInput()
        {
            KeyboardState kb = Keyboard.GetState();

            bool hitK = kb.IsKeyDown(Keys.S);
            bool hitS = kb.IsKeyDown(Keys.K);
            bool hitSpace = kb.IsKeyDown(Keys.Space);

            if (hitS || hitK || hitSpace)
            {
                for (int i = 0; i < _enemies.Count; i++)
                {
                    var enemy = _enemies[i];
                    if (!enemy.IsAlive || Math.Abs(enemy.Position.X - HIT_LINE_X) > PERFECT_RANGE)
                        continue;

                    bool hitSuccess = false;

                    if (hitSpace)
                    {
                        hitSuccess = true;
                        _character.PlayDoubleAttack();
                    }
                    else if (hitS && enemy.Row == 0)
                    {
                        hitSuccess = true;
                        _character.PlayAttack(0);
                    }
                    else if (hitK && enemy.Row == 1)
                    {
                        hitSuccess = true;
                        _character.PlayAttack(1);
                    }

                    if (hitSuccess)
                    {
                        enemy.OnHit();
                        _hitCount++;
                        _score += 100;
                        _audioManager.PlayRandomHitSound();
                        break;
                    }

                }
            }
        }
    }
}
