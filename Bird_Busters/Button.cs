using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bird_Busters
{
    internal class Button
    {
        // To avoid overlapping buttons
        private static int _activeButtons = 0;
        private bool _interacted = false;
        // ------------------------------

        private static int _buttonCount = 0;
        private int _buttonID = 0;

        private Delegate _click;
        private Timer _hoverTweenTimer = new(0);
        private Timer _hoverReleaseTweenTimer = new(0);
        private Timer _pressTweenTimer = new(0);

        private Texture2D _texture;
        private Texture2D _hoverTexture;
        private Texture2D _pressedTexture;
        private Rectangle _rectangle;
        private Rectangle _originalRectangle;
        private float _scale = 1f;
        private float _savedScale = 1f;
        private string _text;
        private SpriteFont _font;
        private Color _textColor;
        private Color _textHoverColor;
        private Color _textPressedColor;
        private Vector2 _textDimension;
        private float textScale = 1.0f;


        private SoundEffect _clickSound;
        private SoundEffect _hoverSound;

        //Anchor Point of button:
        //_________
        //|   x   |
        //|       |
        //|       | 
        //|_______|
        //x = Anchor Point -> Center of the top of the button




        public Button(Vector2 position, Vector2 textureDim, Texture2D texture, Texture2D hoverTexture, Texture2D pressedTexture,
            string text, SpriteFont font, Color textColor, Color textHoverColor, Color textPressedColor)
        {
            _texture = texture;
            _hoverTexture = hoverTexture;
            _pressedTexture = pressedTexture;
            _text = text;
            _font = font;
            _textColor = textColor;
            _textHoverColor = textHoverColor;
            _textPressedColor = textPressedColor;
            _rectangle = new Rectangle((int)(position.X - (textureDim.X / 2f)), (int)position.Y, (int)textureDim.X, (int)textureDim.Y);
            _originalRectangle = _rectangle;
            _textDimension = _font.MeasureString(_text);
            _buttonID = _buttonCount;
            _buttonCount++;

            _clickSound = Game1.BtnClickSfx;
            _hoverSound = Game1.BtnHoverSfx;

            if (_rectangle.X < 0 || _rectangle.Y < 0 || _rectangle.X + _rectangle.Width > Game1.Width ||
                _rectangle.Y + _rectangle.Height > Game1.Height)
            {
                Debug.WriteLine("Button " + _buttonID + " is outside of the screen!");
            }
        }

        /// <summary>
        /// Checks if the mouse is hovering over the button. Use this before IsPressed to avoid unnecessary calls.
        /// Check mouse position with Mouse.GetState().Position and only once before checking all buttons.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsHovering(Vector2 mousePosition)
        {
            if (_originalRectangle.Contains(mousePosition))
            {
                if (!_interacted)
                {
                    _interacted = true;
                    _activeButtons++;
                    _hoverSound.Play(Game1.MenuSFXVolume / 100f, 0, 0);
                    _hoverTweenTimer = new Timer(150, SaveScale);
                }
                return true;
            }
            if (_interacted)
            {
                _interacted = false;
                if (_scale > 1f)
                {
                    _hoverReleaseTweenTimer = new Timer(150);
                }
                _activeButtons--;
            }
            return false;
        }

        /// <summary>
        /// Use this after you checked if the button is hovering, to avoid unnecessary calls
        /// </summary>
        /// <param name="isLeftButtonPressed"></param>
        /// <returns>true if the button was pressed and false else</returns>
        private bool IsPressed(bool isLeftButtonPressed)
        {
            return isLeftButtonPressed;
        }

        /// <summary>
        /// Use this to check if the button was clicked to execute an action in the menu manager.
        /// </summary>
        /// <param name="currentMousePosition"></param>
        /// <param name="wasLeftButtonPressed"></param>
        /// <param name="isLeftButtonReleased"></param>
        /// <returns>True if clicked, false if not clicked.</returns>
        private bool IsClicked(Vector2 currentMousePosition, bool wasLeftButtonPressed, bool isLeftButtonReleased)
        {
            return _originalRectangle.Contains(currentMousePosition) && wasLeftButtonPressed && isLeftButtonReleased;
        }

        public void Update(Vector2 mousePosition, bool wasLeftButtonPressed, bool isLeftButtonReleased)
        {
            //Print button state with id
            //Debug.WriteLine("Button " + _buttonID + " is hovering: " + IsHovering(mousePosition) + " and pressed: " + IsPressed(mousePosition, isLeftButtonPressed));
            //Debug.WriteLine("Active buttons: " + _activeButtons);
            if (IsClicked(mousePosition, wasLeftButtonPressed, isLeftButtonReleased))
            {
                _clickSound.Play(Game1.MenuSFXVolume / 100f, 0, 0);
                Debug.WriteLine("Button " + _buttonID + " was clicked.");
                _click?.DynamicInvoke(); // execute click action if click is not null
            }

            _hoverTweenTimer.Update();
            _hoverReleaseTweenTimer.Update();
            _pressTweenTimer.Update();

            if (_hoverTweenTimer.IsRunning())
            {
                SetRectangleScale(1 + easeOutBack(_hoverTweenTimer.GetProgress() * 0.03f));
            }

            if (_hoverReleaseTweenTimer.IsRunning())
            {
                if (_hoverTweenTimer.IsRunning())
                {
                    _hoverTweenTimer.Pause();
                    SaveScale();
                }
                SetRectangleScale(_savedScale - easeOutBack((_hoverTweenTimer.GetProgress() * _hoverReleaseTweenTimer.GetProgress()) * 0.03f));
            }

            if (_pressTweenTimer.IsRunning())
            {
                SetRectangleScale(_savedScale - ((_savedScale - 0.95f) * _pressTweenTimer.GetProgress()));
            }

            if (IsHovering(mousePosition) && IsPressed(wasLeftButtonPressed))
            {
                _hoverReleaseTweenTimer.Pause();
                _hoverTweenTimer.Pause();
                if (_pressTweenTimer.IsRunning() == false)
                {
                    _pressTweenTimer = new Timer(34);
                    SaveScale();
                }
            }
            else if (_scale < 1f)
            {
                SetRectangleScale(1f);
                _pressTweenTimer.Pause();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 mousePosition, bool isLeftButtonPressed)
        {
            Vector2 position = new Vector2(_rectangle.X + _rectangle.Width / 2f - _textDimension.X / 2f,
                               _rectangle.Y + _rectangle.Height / 2f - _textDimension.Y / 2f);
            if (IsHovering(mousePosition))
            {
                if (IsPressed(isLeftButtonPressed))
                {
                    spriteBatch.Draw(_pressedTexture, _rectangle, Color.White);
                    spriteBatch.DrawString(_font, _text, position, _textPressedColor);
                }
                else
                {
                    spriteBatch.Draw(_hoverTexture, _rectangle, Color.White);
                    spriteBatch.DrawString(_font, _text, position, _textHoverColor);
                }
            }
            else
            {
                spriteBatch.Draw(_texture, _rectangle, Color.White);
                spriteBatch.DrawString(_font, _text, position, _textColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }
        }

        public string GetTextAndId()
        {
            return _text + " | " + _buttonID;
        }

        public void SetClick(Delegate del)
        {
            _click = del;
        }

        public static void ResetConflicts()
        {
            _activeButtons = 0;
        }

        public static bool IsButtonConflict()
        {
            return _activeButtons > 1;
        }

        public void SetRectangleScale(float scale)
        {
            _scale = scale;
            if (scale.Equals(1f))
            {
                _rectangle = _originalRectangle;
                return;
            }
            _rectangle = new Rectangle((int)Math.Round(_originalRectangle.X - (_originalRectangle.Width * scale - _originalRectangle.Width) / 2f, 0), (int)Math.Round(_originalRectangle.Y - (_originalRectangle.Height * scale - _originalRectangle.Height) / 2f, 0), (int)Math.Round(_originalRectangle.Width * scale, 0), (int)Math.Round(_originalRectangle.Height * scale, 0));
        }

        private void SaveScale()
        {
            _savedScale = _scale;
        }

        private float easeOutBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
        }

        public void SetText(string text)
        {
            _text = text;
            _textDimension = _font.MeasureString(_text);
        }

        public void SetButtonTextScale(float scale)
        {
            textScale = scale;
        }
    }
}
