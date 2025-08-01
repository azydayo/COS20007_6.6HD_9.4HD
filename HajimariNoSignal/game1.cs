using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace HajimariNoSignal
{
    public struct ResultData
    {
        public int Score;
        public int HitCount;
        public int MissCount;

        public string GetRank()
        {
            if (MissCount <= 5) return "S";
            if (MissCount <= 8) return "A";
            if (MissCount <= 20) return "B";
            if (MissCount <= 30) return "C";
            return "D";
        }
    }



    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MainMenu _mainMenu;
        private BeatMapMenu _beatMapMenu;

        private Texture2D _entryTexture;
        private Texture2D _bottomSelectionBar;
        public SpriteFont _font;
        private List<Texture2D> _menuFrames;
        private Texture2D _startButton;
        private CustomCursor _customCursor;
        private Texture2D _cursorTexture;

        private BeatMap _activeBeatMap;
        private bool _entryClickedOnce = false;
        private Dictionary<int, string> _stageBackgrounds;

        private Character _character;
        private Texture2D _hitCircleTexture;
        private Texture2D _heartTexture;

        private List<Texture2D> _normalEnemyTextures;
        private Texture2D _fakeEnemyTexture;
        private List<Texture2D> _bombEnemyTextures;



        //private Dictionary<BossEnemy.BossState, List<Texture2D>> _bossAnimations;
        private Dictionary<BossEnemy.BossState, List<Texture2D>> _altBossAnimations;

        private List<Texture2D> _bossSpawnTextures;

        private GameState _currentState = GameState.MainMenu;
        public void ChangeState(GameState newState) => _currentState = newState;

        private AudioManager _audioManager;
        public AudioManager AudioManager => _audioManager;

        public static Microsoft.Xna.Framework.Content.ContentManager StaticContent;

        private Dictionary<int, string> _beatMapPaths;

        private int _lastConfirmedIndex = -1;
        private bool _mouseWasPressed = false;

        private List<(string Title, string Artist)> _labels;

        private GameOverMenu _gameOverMenu;
        private FinalMenu _finalMenu;
        public ResultData ResultData { get; set; }


        public TransitionManager _transitionManager;

        private List<string> _previewPaths;

        public Random _random = new Random();


        public static Game1 Instance;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.PreferredBackBufferWidth = 2284;
            _graphics.PreferredBackBufferHeight = 1536;
            Instance = this;

        }

        

        protected override void LoadContent()
        {
            StaticContent = Content;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _transitionManager = new TransitionManager(this, GraphicsDevice);

            _bottomSelectionBar = Content.Load<Texture2D>("Assets/chrono_selection");
            _entryTexture = Content.Load<Texture2D>("Assets/menu_button2x");
            _font = Content.Load<SpriteFont>("Arial");
            _cursorTexture = Content.Load<Texture2D>("skin/game-scanner");
            _startButton = Content.Load<Texture2D>("skin/menu_enter_button-1_new");
            _hitCircleTexture = Content.Load<Texture2D>("skin/hitcircle");

            _menuFrames = new List<Texture2D>();
            for (int i = 1; i <= 30; i++)
                _menuFrames.Add(Content.Load<Texture2D>($"Menu_Images/menu-bg({i})"));

            _mainMenu = new MainMenu(this, _menuFrames, _startButton);
            _customCursor = new CustomCursor(_cursorTexture);

            _audioManager = new AudioManager();
            _audioManager.LoadSFX("click", Content.Load<SoundEffect>("Audio/click_sound1"));
            _audioManager.LoadHitSounds(16);


            _labels = new List<(string Title, string Artist)>
            {
                ("Zero", "Ado"),
                ("Haru", "Yorushika"),
                ("IDOL", "YOASOBI"),
                ("Hajimari no Signal", "Mejiro McQueen (CV: Onishi Saori)"),
                ("Test", "Nguyen Thien Khanh"),
                ("Sakura Biyori and Time Machine", "Ado X Hatsune Miku"),
                ("Flowering Night", "UNDEAD CORPORATION"),
                ("Everything will Freeze", "UNDEAD CORPORATION"),
                ("Nightglow", "Tanya Chua"),
                ("Da Capo", "HOYO-MiX")
            };

            var bgNames = new List<string>
            {
                "wallpaper/2", "wallpaper/10", "wallpaper/5", "wallpaper/MejiroMcQueen",
                "wallpaper/4", "wallpaper/6", "wallpaper/8", "wallpaper/9", "wallpaper/7", "wallpaper/12"
            };

            _previewPaths = new List<string>
            {
                "Audio/Preview/0",
                "Audio/Preview/haru",
                "Audio/Preview/idol",
                "Audio/Preview/haji",
                "Audio/Preview/test",
                "Audio/Preview/sakura",
                "Audio/Preview/flowering",
                "Audio/Preview/freeze",
                "Audio/Preview/nightglow",
                "Audio/Preview/dacapo"
            };

            List<Song> previews = new List<Song>();
            foreach (string path in _previewPaths)
            {
                previews.Add(Content.Load<Song>(path));
            }



            _stageBackgrounds = new Dictionary<int, string> //TO DO ADD MORE STAGE BACKGROUNDS
            {
                { 0, "BeatMap_Assets/Stages/graveyard" },
                { 1, "BeatMap_Assets/Stages/gensokyo" },
                { 2, "BeatMap_Assets/Stages/candyland" },
                { 3, "BeatMap_Assets/Stages/graveyard" },
                { 4, "BeatMap_Assets/Stages/gensokyo" },
                { 5, "BeatMap_Assets/Stages/candyland" },
                { 6, "BeatMap_Assets/Stages/graveyard" },
                { 7, "BeatMap_Assets/Stages/gensokyo" },
                { 8, "BeatMap_Assets/Stages/candyland" },
                { 9, "BeatMap_Assets/Stages/graveyard" }
            };

            Texture2D defaultBG = Content.Load<Texture2D>("wallpaper/1");

            _beatMapMenu = new BeatMapMenu(
                _bottomSelectionBar, _entryTexture, _font,
                _labels, bgNames, defaultBG,
                GraphicsDevice, Content,previews
            );

            //while (_labels.Count < 20)
            // _labels.Add(($"Track {_labels.Count + 1}", "Unknown Artist"));

            _heartTexture = Content.Load<Texture2D>("BeatMap_Assets/Character/heart"); 

            List<Texture2D> idleFrames = LoadFrames2("BeatMap_Assets/Character/idle", 40);


            List<List<Texture2D>> attacks = new();
            for (int a = 1; a <= 4; a++)
            {
                List<Texture2D> atk = new();
                for (int i = 0; i < 29; i++)
                    atk.Add(Content.Load<Texture2D>($"BeatMap_Assets/Character/Attack{a}/frame_{i:D2}_delay-0.02s"));
                attacks.Add(atk);
            }

            List<List<Texture2D>> doubleAttacks = new();

            doubleAttacks.Add(LoadFrames2("BeatMap_Assets/Character/DoubleAttack1", 29));
            doubleAttacks.Add(LoadFrames2("BeatMap_Assets/Character/DoubleAttack2", 29));

            var hurtFrames = LoadFrames2("BeatMap_Assets/Character/Hurt", 18);

            _character = new Character(idleFrames, attacks, doubleAttacks, hurtFrames);

            _normalEnemyTextures = new List<Texture2D>();
            for (int i = 1; i <= 8; i++)
            {
                _normalEnemyTextures.Add(Content.Load<Texture2D>($"Beatmap_Assets/Enemies/Normal/{i}"));
            }
            /*
            _bossAnimations = new()
            {
                { BossEnemy.BossState.Appear, LoadBossFrames("BeatMap_Assets/Enemies/Boss/Appear", 30) },
                { BossEnemy.BossState.Idle, LoadBossFrames("BeatMap_Assets/Enemies/Boss/Idle", 60) },
                { BossEnemy.BossState.AirAttack, LoadBossFrames("BeatMap_Assets/Enemies/Boss/AirAttack", 60) },
                { BossEnemy.BossState.LowAttack, LoadBossFrames("BeatMap_Assets/Enemies/Boss/LowAttack", 60) },
                { BossEnemy.BossState.Exit, LoadBossFrames("BeatMap_Assets/Enemies/Boss/Exit", 30) }
            };
            */

            _altBossAnimations = new()
                {
                    { BossEnemy.BossState.Idle, LoadFrames1("BeatMap_Assets/Enemies/Boss/AltIdle", 14) },
                    { BossEnemy.BossState.AirAttack, LoadFrames1("BeatMap_Assets/Enemies/Boss/AttackAlt", 10) },
                    { BossEnemy.BossState.LowAttack, LoadFrames1("BeatMap_Assets/Enemies/Boss/AttackAlt", 10) },
                    { BossEnemy.BossState.Appear, LoadFrames1("BeatMap_Assets/Enemies/Boss/Appear", 9) },
                    { BossEnemy.BossState.Exit, LoadFrames1("BeatMap_Assets/Enemies/Boss/Exit",9 ) }
                };

            _bossSpawnTextures = new();
            for (int i = 1; i <= 4; i++)
                _bossSpawnTextures.Add(Content.Load<Texture2D>($"BeatMap_Assets/Enemies/BossSpawn/{i}"));

            _bombEnemyTextures = new();
            for (int i = 1; i <= 2; i++)
                _bombEnemyTextures.Add(Content.Load<Texture2D>($"BeatMap_Assets/Enemies/Bomb/{i}"));
            

            _fakeEnemyTexture = Content.Load<Texture2D>("BeatMap_Assets/Enemies/Fake/1");

            Texture2D gameOverBG = Content.Load<Texture2D>("wallpaper/failbg");
            Texture2D gameOverUI = Content.Load<Texture2D>("wallpaper/fail");
            Texture2D backButton = Content.Load<Texture2D>("skin/back");

            _gameOverMenu = new GameOverMenu(gameOverBG, gameOverUI, backButton);

            Texture2D finalBG = Content.Load<Texture2D>("wallpaper/winbg");
            Texture2D finalUI = Content.Load<Texture2D>("wallpaper/win");
            _finalMenu = new FinalMenu(finalBG, finalUI, backButton);


            _audioManager.LoadSFX("hover", Content.Load<SoundEffect>("Audio/SFX/sound-hover"));
            _audioManager.LoadSFX("select", Content.Load<SoundEffect>("Audio/SFX/sound-select"));
            _audioManager.LoadSFX("back", Content.Load<SoundEffect>("Audio/SFX/back-button-click"));






        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_transitionManager.IsActive)
            {
                _transitionManager.Update();
                return; // prevent inputs while fading
            }
            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Update(gameTime);
                    break;
                case GameState.BeatMapMenu:
                    _beatMapMenu.Update(gameTime);

                    int currentSelected = _beatMapMenu.GetSelectedIndex();
                    MouseState mouse = Mouse.GetState();

                    if (currentSelected >= 0 && mouse.LeftButton == ButtonState.Pressed && !_mouseWasPressed)
                    {
                        if (_lastConfirmedIndex == currentSelected)
                        {
                            if (_stageBackgrounds.TryGetValue(currentSelected, out string stagePath))
                            {
                                Texture2D stageBG = Content.Load<Texture2D>(stagePath);

                                string songTitle = _labels[currentSelected].Title;
                                string difficulty = "Normal";
                                string beatmapPath = Path.Combine("Content", "Beatmaps", songTitle, $"{difficulty}.json");

                                _activeBeatMap = new BeatMap(
                                    beatmapPath,
                                    stageBG,
                                    _character,
                                    _hitCircleTexture,
                                    _normalEnemyTextures,
                                    _audioManager,
                                    _altBossAnimations,
                                    _bossSpawnTextures,
                                    _fakeEnemyTexture,
                                    _bombEnemyTextures,
                                    _heartTexture
                                );

                                _audioManager.StopPreview();
                                //ChangeState(GameState.BeatMap);
                                _character.Reset();
                                Game1.Instance._transitionManager.Start(GameState.BeatMap);

                            }
                            _entryClickedOnce = false;
                        }
                        else
                        {
                            _lastConfirmedIndex = currentSelected;
                            _entryClickedOnce = true;
                        }


                    }

                    _mouseWasPressed = mouse.LeftButton == ButtonState.Pressed;
                    break;
                case GameState.BeatMap:
                    _activeBeatMap?.Update(gameTime);
                    break;
                case GameState.GameOver:
                    _gameOverMenu.Update(gameTime);
                    break;
                case GameState.Final:
                    _finalMenu.Update(gameTime);
                    break;

            }

            base.Update(gameTime);
        }

        private List<Texture2D> LoadFrames2(string folderPath, int frameCount)
        {
            List<Texture2D> frames = new();
            for (int i = 0; i < frameCount; i++) //0 BASED INDEX
            {
                string fileName = $"{folderPath}/frame_{i:D2}_delay-0.02s";
                frames.Add(Content.Load<Texture2D>(fileName));
            }
            return frames;
        }

        private List<Texture2D> LoadFrames1(string folderPath, int frameCount)
        {
            List<Texture2D> frames = new();
            for (int i = 1; i <= frameCount; i++) //1 BASED INDEX
            {
                string fileName = $"{folderPath}/frame_{i:D2}_delay-0.01s";
                frames.Add(Content.Load<Texture2D>(fileName));
            }
            return frames;
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch);
                    _transitionManager.Draw(_spriteBatch);

                    break;
                case GameState.BeatMapMenu:
                    _beatMapMenu.Draw(_spriteBatch);
                    _transitionManager.Draw(_spriteBatch);

                    break;
                case GameState.BeatMap:
                    _activeBeatMap?.Draw(_spriteBatch);
                    _transitionManager.Draw(_spriteBatch);

                    break;
                case GameState.GameOver:
                    _gameOverMenu.Draw(_spriteBatch);
                    _transitionManager.Draw(_spriteBatch);
                    break;
                case GameState.Final:
                    _finalMenu.Draw(_spriteBatch);
                    _transitionManager.Draw(_spriteBatch);
                    break;


            }

            _customCursor.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
