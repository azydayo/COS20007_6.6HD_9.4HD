using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace HajimariNoSignal
{
    public class BeatMapMenu
    {
        private Texture2D _overlayTexture;

        private List<BeatMapEntry> _entries;
        private SpriteFont _font;
        private Texture2D _bottomSelectionBar;
        private Texture2D _topUI;


        private List<Texture2D> _backgrounds;
        private Texture2D _defaultBackground;

        private Texture2D _currentBackground;
        private Texture2D _nextBackground;

        private float _backgroundAlpha = 1f;
        private bool _isTransitioning = false;
        private float _transitionSpeed = 0.05f;


        private float _scrollPosition = 0f;
        private float _scrollTarget = 0f;
        private float _scrollSpeed = 0.15f;

        private int _hoveredIndex = -1;
        private int _selectedIndex = -1;
        private int _lastScrollValue = 0;

        private const int ENTRY_HEIGHT = 180;
        private const int ENTRY_OVERLAP = 20;
        private const int CENTER_Y = 768;

        private List<Song> _previewSongs;
        private int _lastPreviewIndex = -1;
        private int _lastHoveredIndex = -1;



        public BeatMapMenu(Texture2D bottomSelectionBar, Texture2D entryTexture, SpriteFont font,
                           List<(string title, string artist)> labels,
                           List<string> backgroundNames,
                           Texture2D defaultBackground,
                           GraphicsDevice graphicsDevice,
                           Microsoft.Xna.Framework.Content.ContentManager content,
                           List<Song> previewSongs)
        {
            _font = font;
            _bottomSelectionBar = bottomSelectionBar;
            _entries = new List<BeatMapEntry>();
            _backgrounds = new List<Texture2D>();
            _defaultBackground = defaultBackground;

            _currentBackground = defaultBackground;
            _previewSongs = previewSongs;


            Texture2D lastValidBackground = defaultBackground;
            _topUI = content.Load<Texture2D>("Assets/TopUI"); // Adjust path if needed


            _overlayTexture = new Texture2D(graphicsDevice, 1, 1);
            _overlayTexture.SetData(new[] { Color.White }); // solid white pixel


            for (int i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                _entries.Add(new BeatMapEntry(entryTexture, label.title, label.artist, 1284f));

                string bgName = (i < backgroundNames.Count) ? backgroundNames[i] : "NONE";

                if (bgName != "NONE")
                {
                    try
                    {
                        var bg = content.Load<Texture2D>(bgName);
                        _backgrounds.Add(bg);
                    }
                    catch
                    {
                        _backgrounds.Add(defaultBackground);
                    }
                }
                else
                {
                    _backgrounds.Add(defaultBackground);
                }

            }
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            int scrollDelta = mouse.ScrollWheelValue - _lastScrollValue;
            _lastScrollValue = mouse.ScrollWheelValue;

            if (scrollDelta != 0)
            {
                float direction = scrollDelta > 0 ? -1f : 1f;
                float maxScroll = _entries.Count - 1;
                _scrollTarget = MathHelper.Clamp(_scrollTarget + direction, 0f, maxScroll);
            }

            _scrollPosition += (_scrollTarget - _scrollPosition) * _scrollSpeed;

            _hoveredIndex = -1;

            for (int i = 0; i < _entries.Count; i++)
            {
                float offset = i - _scrollPosition;
                float y = CENTER_Y + offset * (ENTRY_HEIGHT - ENTRY_OVERLAP);

                Rectangle bounds = new Rectangle(1284, (int)y, 1450, ENTRY_HEIGHT);

                if (bounds.Contains(mouse.Position))
                {
                    _hoveredIndex = i;
                    if (_hoveredIndex != _lastHoveredIndex)
                    {
                        Game1.Instance.AudioManager.PlaySFX("hover");
                        _lastHoveredIndex = _hoveredIndex;
                    }

                    // Only change selection on click
                    if (mouse.LeftButton == ButtonState.Pressed && _selectedIndex != i)
                    {
                        _selectedIndex = i;
                        _nextBackground = _backgrounds[i];
                        _isTransitioning = true;
                        _backgroundAlpha = 1f;
                        Game1.Instance.AudioManager.PlaySFX("select");

                        if (i != _lastPreviewIndex && i < _previewSongs.Count)
                        {
                            Game1.Instance.AudioManager.PlayPreview(_previewSongs[i]);
                            _lastPreviewIndex = i;
                        }
                    }

                }

                _entries[i].Update(i, _scrollPosition, _hoveredIndex == i, _selectedIndex == i);
            }

            // Transition logic OUTSIDE the loop
            if (_isTransitioning)
            {
                _backgroundAlpha -= _transitionSpeed;
                if (_backgroundAlpha <= 0f)
                {
                    _currentBackground = _nextBackground;
                    _backgroundAlpha = 0f;
                    _isTransitioning = false;
                }
            }


        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (_currentBackground != null)
            {
                Rectangle fit = ImageFitter.GetFittingRectangle(_currentBackground, 2284, 1536);
                spriteBatch.Draw(_currentBackground, fit, Color.White);
            }

            // Fade in new background if transitioning
            if (_isTransitioning && _nextBackground != null)
            {
                Rectangle fitNext = ImageFitter.GetFittingRectangle(_nextBackground, 2284, 1536);
                spriteBatch.Draw(_nextBackground, fitNext, Color.White * (1f - _backgroundAlpha));
            }

            spriteBatch.Draw(_overlayTexture, new Rectangle(0, 0, 2284, 1536), Color.Black * 0.5f); // 50% opacity

            for (int i = 0; i < _entries.Count; i++)
            {
                float offset = i - _scrollPosition;
                float y = CENTER_Y + offset * (ENTRY_HEIGHT - ENTRY_OVERLAP);

                bool isSelected = (i == _selectedIndex);
                float scale = MathHelper.Clamp(1f - Math.Abs(offset) * 0.04f, 0.85f, 1f);
                float opacity = isSelected ? 1f : 0.85f;

                _entries[i].Draw(spriteBatch, _font, y, scale, opacity);
            }
            
            spriteBatch.Draw(_bottomSelectionBar, new Rectangle(0, 0, 2284, 1536), Color.White);
            spriteBatch.Draw(_topUI, new Vector2(0, 0), Color.White);
        }

        public int GetSelectedIndex()
        {
            return _selectedIndex;
        }
    }
}
