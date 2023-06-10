using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    internal class Button
    {
        // To avoid overlapping buttons
        private static int _activeButtons = 0;
        private bool _interacted = false;
        // ------------------------------

        private static int _buttonCount = 0;
        private int _buttonID = 0;

        private Delegate click;

        private Texture2D _texture;
        private Texture2D _hoverTexture;
        private Texture2D _pressedTexture;
        private Rectangle _rectangle;
        private string _text;
        private SpriteFont _font;
        private Color _textColor;
        private Color _textHoverColor;
        private Color _textPressedColor;
        private Vector2 _textDimension;

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
            _rectangle = new Rectangle((int)position.X-((int)textureDim.X/2), (int)position.Y, (int)textureDim.X, (int)textureDim.Y);
            _textDimension = _font.MeasureString(_text);
            _buttonID = _buttonCount;
            _buttonCount++;
        }

        /// <summary>
        /// Checks if the mouse is hovering over the button. Use this before IsPressed to avoid unnecessary calls.
        /// Check mouse position with Mouse.GetState().Position and only once before checking all buttons.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsHovering(Vector2 mousePosition)
        {
            if (_rectangle.Contains(mousePosition))
            {
                if (!_interacted)
                {
                    _interacted = true;
                    _activeButtons++;
                }
                return true;
            }
            else
            {
                if (_interacted)
                {
                    _interacted = false;
                    _activeButtons--;
                }
                return false;
            }

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
            return _rectangle.Contains(currentMousePosition) && wasLeftButtonPressed && isLeftButtonReleased;
        }

        public void Update(Vector2 mousePosition, bool wasLeftButtonPressed, bool isLeftButtonReleased)
        {
            //Print button state with id
            //Debug.WriteLine("Button " + _buttonID + " is hovering: " + IsHovering(mousePosition) + " and pressed: " + IsPressed(mousePosition, isLeftButtonPressed));
            //Debug.WriteLine("Active buttons: " + _activeButtons);
            if (IsClicked(mousePosition,wasLeftButtonPressed, isLeftButtonReleased))
            {
                Debug.WriteLine("Button " + _buttonID + " was clicked.");
                click?.DynamicInvoke(); // execute click action if click is not null
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
                spriteBatch.DrawString(_font, _text, position, _textColor);
            }
        }

        public string GetTextAndId()
        {
            return _text + " | " + _buttonID;
        }

        public void SetClick(Delegate del)
        {
            click = del;
        }

        public static void ResetConflicts()
        {
            _activeButtons = 0;
        }

        public static bool IsButtonConflict()
        {
            return _activeButtons > 1;
        }

        
    }
}
