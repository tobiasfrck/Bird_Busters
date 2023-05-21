using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    //Button actions are defined here
    internal class MenuManager
    {
        private SpriteBatch _spriteBatch;

        private Texture2D _menuBackground;
        private Texture2D _settingsBackground;
        private Texture2D _gameBackground;
        private Texture2D _gameEndedBackground;

        private List<Button> _menuButtons = new();
        private List<Button> _settingsButtons = new();
        private List<Button> _gameButtons = new();
        private List<Button> _gameEndedButtons = new();
        private int _state = 2; // 0 = menu, 1 = settings, 2 = game, 3 = game ended

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public MenuManager(SpriteBatch spriteBatch,Texture2D menuBackground, Texture2D settingsBackground, Texture2D gameBackground, Texture2D gameEndedBackground)
        {
            _spriteBatch = spriteBatch;
            _menuBackground = menuBackground;
            _settingsBackground = settingsBackground;
            _gameBackground = gameBackground;
            _gameEndedBackground = gameEndedBackground;

            // Do NOT change the order of the buttons; This WILL break the code. lol
            createMainMenu(); 
        }

        public void Update()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
            Vector2 mousePosition = _currentMouseState.Position.ToVector2();
            switch (_state)
            {
                case 0:
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Update(mousePosition,_previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
            }

            if (Button.IsButtonConflict())
            {
                Debug.WriteLine("Button Conflict.");
            }
        }

        public void Draw()
        {
            switch (_state)
            {
                case 0:
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Draw(_spriteBatch,_currentMouseState.Position.ToVector2(),_currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 1:
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 2:
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 3:
                    foreach (Button btn in _gameEndedButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                default:
                    break;
            }
        }

        public void createMainMenu()
        {
            _menuButtons.Add(new Button(new Vector2(Game1.Width/2f, 25), new Vector2(640,360),Game1.ButtonPlaceholder, Game1.ButtonHoverPlaceholder, Game1.ButtonPressedPlaceholder, "Start Game", Game1.TestFont, Color.Black, Color.Black, Color.Black));
            _menuButtons.Add(new Button(new Vector2(Game1.Width / 2f, 400), new Vector2(640, 360),Game1.ButtonPlaceholder, Game1.ButtonHoverPlaceholder, Game1.ButtonPressedPlaceholder, "Settings", Game1.TestFont, Color.Black, Color.Black, Color.Black));
            _menuButtons.Add(new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360),Game1.ButtonPlaceholder, Game1.ButtonHoverPlaceholder, Game1.ButtonPressedPlaceholder, "Exit", Game1.TestFont, Color.Black, Color.Black, Color.Black));
        }

        public int GetState() => _state;
        public void SetState(int state) => _state = state;
    }
}
