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
    public enum GameState
    {
        Menu = 0,
        Settings = 1,
        Game = 2,
        GameEnded = 3
    }

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
        private GameState _state = GameState.Menu; // Menu, Settings, Game, GameEnded

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
                case GameState.Menu:
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case GameState.Settings:
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case GameState.Game:
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Update(mousePosition, _previousMouseState.LeftButton == ButtonState.Pressed, _currentMouseState.LeftButton == ButtonState.Released);
                    }
                    break;
                case GameState.GameEnded:
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
                case GameState.Menu:
                    if (_menuBackground!=null)
                    {
                        _spriteBatch.Draw(_menuBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.Settings:

                    if (_settingsBackground!=null)
                    {
                        _spriteBatch.Draw(_settingsBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.Game:
                    if (_gameBackground != null)
                    {
                        _spriteBatch.Draw(_gameBackground, _fullScreen, Color.White);
                    }
                    if (Level.GetDebugMode())
                    {
                        Vector2 textDim = Game1.TestFont.MeasureString(Level.GetDebugText());
                        _spriteBatch.DrawString(Game1.TestFont, Level.GetDebugText(), new Vector2((Game1.Width / 2f) - (textDim.X / 2f), 900), Color.Red);
                    }
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.GameEnded:
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

        private void CreateMainMenu()
        {
            Button startGame = new Button(new Vector2(Game1.Width / 2f, 25), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Start Game", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button settings = new Button(new Vector2(Game1.Width / 2f, 400), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Settings", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button exitGame = new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Exit", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            _menuButtons.Add(startGame);
            _menuButtons.Add(settings);
            _menuButtons.Add(exitGame);
            startGame.SetClick(SwitchToGame); //action for button 0
            settings.SetClick(SwitchToSettings); //action for button 1
            exitGame.SetClick(CloseGame);
        }

        private void CreateSettingsMenu()
        {
            Button toMenu = new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Back", Game1.TestFont, Color.Black, Color.Green, Color.Red);
            _settingsButtons.Add(toMenu);
            toMenu.SetClick(SwitchToMenu);
        }

        public GameState GetState() => _state;
        private void SetState(GameState state) => _state = state;

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
            SetState(GameState.Settings);
        }

        public void SwitchToGame()
        {
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            Button.ResetConflicts();
            SetState(GameState.Game);
        }

        public void SwitchToEndscreen()
        {
            Button.ResetConflicts();
            SetState(GameState.GameEnded);
        }

        public void CloseGame()
        {
            Game1.CloseGame = true;
        }
    }
}
