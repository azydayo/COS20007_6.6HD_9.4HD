using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HajimariNoSignal
{
    public class TransitionManager
    {
        private enum TransitionState { Done, FadingOut, FadingIn }

        private TransitionState _state = TransitionState.Done;
        private float _alpha = 0f;
        private float _speed = 10f; // speed of fade

        private GameState _nextState;
        private Game1 _game;
        private Texture2D _pixel;

        public bool IsActive => _state != TransitionState.Done;

        public TransitionManager(Game1 game, GraphicsDevice graphics)
        {
            _game = game;
            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { Color.Black });
        }

        public void Start(GameState nextState)
        {
            _state = TransitionState.FadingOut;
            _nextState = nextState;
        }


        public void Update()
        {
            if (_state == TransitionState.FadingOut)
            {
                _alpha += _speed;
                if (_alpha >= 255)
                {
                    _alpha = 255;

                    _game.ChangeState(_nextState);

                    _state = TransitionState.FadingIn;
                }
            }
            else if (_state == TransitionState.FadingIn)
            {
                _alpha -= _speed;
                if (_alpha <= 0)
                {
                    _alpha = 0;
                    _state = TransitionState.Done;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (_state == TransitionState.Done) return;

            Color fadeColor = Color.Black * (_alpha / 255f);
            spriteBatch.Draw(_pixel, new Rectangle(0, 0, 2284, 1536), fadeColor);
        }
    }
}
