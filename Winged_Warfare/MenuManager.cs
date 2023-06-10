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
        private readonly Rectangle _fullScreen = new(0, 0, Game1.Width, Game1.Height);

        private Texture2D _menuBackground;
        private Texture2D _settingsBackground;
        private Texture2D _gameBackground;
        private Texture2D _gameEndedBackground;

        private List<Button> _menuButtons = new();
        private List<Button> _settingsButtons = new();
        private List<Button> _gameButtons = new();
        private List<Button> _gameEndedButtons = new();
        private int _state = 0; // 0 = menu, 1 = settings, 2 = game, 3 = game ended

        private float _horizontalCenter = 0;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public MenuManager(SpriteBatch spriteBatch, Texture2D menuBackground, Texture2D settingsBackground, Texture2D gameBackground, Texture2D gameEndedBackground)
        {
            _spriteBatch = spriteBatch;
            _menuBackground = menuBackground;
            _settingsBackground = settingsBackground;
            _gameBackground = gameBackground;
            _gameEndedBackground = gameEndedBackground;

            // Do NOT change the order of the buttons; This WILL break the code. lol
            CreateMainMenu();
            CreateSettingsMenu();

            foreach (Button button in _menuButtons)
            {
                Debug.WriteLine(button.GetTextAndId());
            }
            foreach (Button button in _settingsButtons)
            {
                Debug.WriteLine(button.GetTextAndId());
            }
            foreach (Button button in _gameButtons)
            {
                Debug.WriteLine(button.GetTextAndId());
            }
            foreach (Button button in _gameEndedButtons)
            {
                Debug.WriteLine(button.GetTextAndId());
            }
            _horizontalCenter = Game1.Width / 2f;
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
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case 1:
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case 2:
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case 3:
                    foreach (Button btn in _gameEndedButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                default:
                    break;
            }

            if (Button.IsButtonConflict())
            {
                Debug.WriteLine("Button Conflict.");
            }
            _horizontalCenter = Game1.Width / 2f;
        }

        public void Draw()
        {
            switch (_state)
            {
                case 0:
                    if (_menuBackground!=null)
                    {
                        _spriteBatch.Draw(_menuBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 1:

                    if (_settingsBackground!=null)
                    {
                        _spriteBatch.Draw(_settingsBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 2:
                    if (_gameBackground != null)
                    {
                        _spriteBatch.Draw(_gameBackground, _fullScreen, Color.White);
                    }
                    if (Level.GetDebugMode())
                    {
                        Vector2 textdim = Game1.TestFont.MeasureString(Level.GetDebugText());
                        _spriteBatch.DrawString(Game1.TestFont, Level.GetDebugText(), new Vector2((Game1.Width / 2f) - (textdim.X / 2f), 900), Color.Red);
                    }
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case 3:
                    if (_gameEndedBackground!=null)
                    {
                        _spriteBatch.Draw(_gameEndedBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _gameEndedButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                default:
                    Debug.WriteLine("[ERROR]: Invalid state.");
                    SetState(0);
                    break;
            }
        }

        public void CreateMainMenu()
        {
            Button StartGame = new Button(new Vector2(Game1.Width / 2f, 25), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Start Game", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button Settings = new Button(new Vector2(Game1.Width / 2f, 400), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Settings", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button ExitGame = new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Exit", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            _menuButtons.Add(StartGame);
            _menuButtons.Add(Settings);
            _menuButtons.Add(ExitGame);
            StartGame.SetClick(SwitchToGame); //action for button 0
            Settings.SetClick(SwitchToSettings); //action for button 1
            ExitGame.SetClick(CloseGame);
        }

        public void CreateSettingsMenu()
        {
            Button ToMenu = new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Back", Game1.TestFont, Color.Black, Color.Green, Color.Red);
            _settingsButtons.Add(ToMenu);
            ToMenu.SetClick(SwitchToMenu);
        }

        public int GetState() => _state;
        public void SetState(int state) => _state = state;

        //---------------------------
        //All actions for the buttons
        public void SwitchToMenu()
        {
            Button.ResetConflicts();
            SetState(0);
        }

        public void SwitchToSettings()
        {
            Button.ResetConflicts();
            SetState(1);
        }

        public void SwitchToGame()
        {
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            Button.ResetConflicts();
            SetState(2);
        }

        public void SwitchToEndscreen()
        {
            Button.ResetConflicts();
            SetState(3);
        }

        public void CloseGame()
        {
            Game1.Exit = true;
        }
    }
}
