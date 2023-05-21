using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Winged_Warfare
{
    internal class Button
    {
        private static int _buttonCount = 0;
        private int _buttonID = 0;
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

        public Button(Vector2 position, Texture2D texture, Texture2D hoverTexture, Texture2D pressedTexture,
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
            _rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            _textDimension = _font.MeasureString(_text);
            _buttonCount++;
            _buttonID = _buttonCount;
        }

        /// <summary>
        /// Checks if the mouse is hovering over the button. Use this before IsPressed to avoid unnecessary calls.
        /// Check mouse position with Mouse.GetState().Position and only once before checking all buttons.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public bool IsHovering(Vector2 mousePosition)
        {
            return _rectangle.Contains(mousePosition);
        }

        /// <summary>
        /// Use this after you checked if the button is hovering, to avoid unnecessary calls
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="isLeftButtonPressed"></param>
        /// <returns>true if the button was pressed and false else</returns>
        public bool IsPressed(Vector2 mousePosition, bool isLeftButtonPressed)
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
        public bool IsClicked(Vector2 currentMousePosition, bool wasLeftButtonPressed, bool isLeftButtonReleased)
        {
            return _rectangle.Contains(currentMousePosition) && wasLeftButtonPressed && isLeftButtonReleased;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 mousePosition, bool isLeftButtonPressed)
        {
            Vector2 position = new Vector2(_rectangle.X + _rectangle.Width / 2f - _textDimension.X / 2f,
                               _rectangle.Y + _rectangle.Height / 2f - _textDimension.Y / 2f);
            if (IsHovering(mousePosition))
            {
                if (IsPressed(mousePosition, isLeftButtonPressed))
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
            return _text+ _buttonID;
        }
    }
}
