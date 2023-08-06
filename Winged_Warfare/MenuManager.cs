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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly Rectangle _fullScreen = new(0, 0, Game1.Width, Game1.Height);
        private Viewport Viewport = new(0, 0, Game1.Width, Game1.Height);

        private Texture2D _menuBackground;
        private Texture2D _settingsBackground;
        private Texture2D _gameBackground;
        private Texture2D _gameEndedBackground;
        private Texture2D _blankTexture;

        private List<Button> _menuButtons = new();
        private List<Button> _settingsButtons = new();
        public List<Button> _gameButtons = new();
        private List<Button> _gameEndedButtons = new();
        public GameState _state = GameState.Menu; // Menu, Settings, Game, GameEnded

        private float _horizontalCenter = 0;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private bool _gameNeedsReset = false;

        public MenuManager(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Texture2D menuBackground, Texture2D settingsBackground, Texture2D gameBackground, Texture2D gameEndedBackground)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _menuBackground = menuBackground;
            _settingsBackground = settingsBackground;
            _gameBackground = gameBackground;
            _gameEndedBackground = gameEndedBackground;
            _blankTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _horizontalCenter = Game1.Width / 2f;

            // Do NOT change the order of the buttons; This WILL break the code. lol
            CreateMainMenu();
            CreateSettingsMenu();
            CreateGameEndedMenu();

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
                    Debug.WriteLine("[ERROR]: Invalid state.");
                    SetState(0);
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
                    if (_menuBackground != null)
                    {
                        _spriteBatch.Draw(_menuBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _menuButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.Settings:
                    if (_settingsBackground != null)
                    {
                        _spriteBatch.Draw(_settingsBackground, _fullScreen, Color.White);
                    }
                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.Game:
                    //Vector2 t = WorldToScreen(new Vector3(0, 0, 0));
                    //_spriteBatch.Draw(Game1.HUDAmmoEmpty,t, Color.White);
                    // Margin for all HUD elements on the right side of the screen
                    int xOffset = Game1.Width - 10;

                    //TODO: Clean unused code if you are sure it is not needed! So probably later.
                    // Would draw the game background here but currently not used
                    if (_gameBackground != null)
                    {
                        _spriteBatch.Draw(_gameBackground, _fullScreen, Color.White);
                    }
                    DrawDebug();

                    // Draw Ammunition HUD
                    int magDisplaySize = 100; //icon size
                    int magDisplayYPosition = Game1.Height - 110;
                    int magDisplayDistanceBetween = -110; //distance between icons
                    for (int i = 0; i < BulletHandler.GetMagazinSize(); i++)
                    {
                        int magDisplayXPosition = (i + 1) * magDisplayDistanceBetween + xOffset;
                        //Draw unavailable bullets while reloading
                        if (BulletHandler.IsReloading() == true)
                        {
                            _spriteBatch.Draw(Game1.HUDAmmoReloading, new Rectangle(magDisplayXPosition, magDisplayYPosition, magDisplaySize, magDisplaySize), Color.White);
                        }
                        //Draw available bullets
                        else if (i < BulletHandler.GetAvailableShots() && BulletHandler.IsReloading() == false)
                        {
                            _spriteBatch.Draw(Game1.HUDAmmo, new Rectangle(magDisplayXPosition, magDisplayYPosition, magDisplaySize, magDisplaySize), Color.White);
                        }
                        else //Draw unavailable bullets
                        {
                            _spriteBatch.Draw(Game1.HUDAmmoEmpty, new Rectangle(magDisplayXPosition, magDisplayYPosition, magDisplaySize, magDisplaySize), Color.White);
                        }
                    }

                    //Draw Highscore HUD
                    float highscoreTextXSize = Game1.TestFont.MeasureString("Highscore: " + Score.GetHighscore()).X;
                    float highscoreTextYSize = Game1.TestFont.MeasureString("Highscore: " + Score.GetHighscore()).Y;
                    _spriteBatch.DrawString(Game1.TestFont, "Highscore: " + Score.GetHighscore(), new Vector2(xOffset - highscoreTextXSize, 10), Color.White);

                    // Draw Score HUD
                    float scoreTextSize = Game1.TestFont.MeasureString("Score: " + Score.GetScore()).X;

                    _spriteBatch.DrawString(Game1.TestFont, "Score: " + Score.GetScore(), new Vector2(xOffset - scoreTextSize, 10 + highscoreTextYSize), Color.White);

                    // TODO: Draw remaining time HUD
                    _spriteBatch.DrawString(Game1.TestFont, "Time: " + Game1._gameTimer?.GetSeconds(), new Vector2(10, 10), Color.White);

                    //Some kind of buttons during gameplay; currently not used
                    foreach (Button btn in _gameButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.GameEnded:
                    if (_gameEndedBackground != null)
                    {
                        _spriteBatch.Draw(_gameEndedBackground, _fullScreen, Color.White);
                    }

                    Vector2 gameEndedDim = Game1.TestFont.MeasureString("Game Ended");
                    _spriteBatch.DrawString(Game1.TestFont, "Game Ended", new Vector2(_horizontalCenter-gameEndedDim.X/2, 25), Color.White);
                    Vector2 scoreDim = Game1.TestFont.MeasureString("Score: " + Score.GetScore());
                    _spriteBatch.DrawString(Game1.TestFont, "Score: " + Score.GetScore(), new Vector2(_horizontalCenter - scoreDim.X / 2, 100), Color.White);
                    Vector2 highscoreDim = Game1.TestFont.MeasureString("Highscore: " + Score.GetHighscore());
                    _spriteBatch.DrawString(Game1.TestFont, "Highscore: " + Score.GetHighscore(), new Vector2(_horizontalCenter - highscoreDim.X / 2, 150), Color.White);

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

        [Conditional("DEBUG")]
        private void DrawDebug()
        {
            if (Level.GetDebugMode())
            {
                Vector2 textDim = Game1.TestFont.MeasureString(Level.GetDebugText());
                _spriteBatch.DrawString(Game1.TestFont, Level.GetDebugText(),
                    new Vector2((Game1.Width / 2f) - (textDim.X / 2f), 900), Color.Red);
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
            Button toMenu = new Button(new Vector2(Game1.Width / 2f, 785), new Vector2(640, 360), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Back", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            _settingsButtons.Add(toMenu);
            toMenu.SetClick(SwitchToMenu);
        }

        private void CreateGameEndedMenu()
        {
            Vector2 backSize = Game1.TestFont.MeasureString("Return to menu");
            Vector2 restartSize = Game1.TestFont.MeasureString("Play another round");
            Vector2 backPadding = new Vector2(40, 40);
            Vector2 restartPadding = new Vector2(40, 40);

            Button toMenu = new Button(new Vector2(_horizontalCenter + backPadding.X, 880) - backPadding, backSize + backPadding, Game1.Grey80, Game1.Grey40, _blankTexture, "Return to menu", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button restart = new Button(new Vector2(_horizontalCenter + restartPadding.X, 1000) - restartPadding, restartSize + restartPadding, Game1.Grey80, Game1.Grey40, _blankTexture, "Play another round", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            _gameEndedButtons.Add(toMenu);
            _gameEndedButtons.Add(restart);
            toMenu.SetClick(SwitchToMenu);
            restart.SetClick(SwitchToGame);
        }

        public GameState GetState() => _state;
        private void SetState(GameState state) => _state = state;

        //---------------------------
        //All actions for the buttons
        public void SwitchToMenu()
        {
            Game1._gameTimer?.Pause();
            Button.ResetConflicts();
            SetState(GameState.Menu);
            Debug.WriteLine("Switched to menu state.");
        }

        public void SwitchToSettings()
        {
            Button.ResetConflicts();
            SetState(GameState.Settings);
            Debug.WriteLine("Switched to settings state.");
        }

        public void SwitchToGame()
        {
            Game1._gameTimer = new Timer(70000, SetGameNeedsReset);
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            Button.ResetConflicts();
            SetState(GameState.Game);
            Debug.WriteLine("Switched to game state.");
            Game1.Instance.ResetGame();
        }

        public void SwitchToEndscreen()
        {
            Game1._gameTimer?.Pause();
            Button.ResetConflicts();
            SetState(GameState.GameEnded);
            Debug.WriteLine("Switched to endscreen state.");
        }

        public void EnableFullscreen()
        {
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        public void DisableFullscreen()
        {
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        private void SetGameNeedsReset()
        {
            _gameNeedsReset = true;
        }

        public bool NeedsReset()
        {
            if (_gameNeedsReset == true)
            {
                _gameNeedsReset = false;
                return true;
            }
            return false;
        }


        public void CloseGame()
        {
            Game1.CloseGame = true;
        }

        public static Vector2 WorldToScreen(Vector3 worldPosition)
        {
            // Transform the world position by the view and projection matrices
            Vector3 transformedPosition = Vector3.Transform(worldPosition, Player.ViewMatrix * Player.ProjectionMatrix);

            // Normalize the transformed position
            Vector2 normalizedPosition = new Vector2(transformedPosition.X / transformedPosition.Z, transformedPosition.Y / transformedPosition.Z);

            // Convert the normalized position to screenspace
            Vector2 screenspacePosition = new Vector2(
                (normalizedPosition.X + 1) * 0.5f * Game1.Width,
                (1 - normalizedPosition.Y) * 0.5f * Game1.Height);

            return screenspacePosition;
        }

    }
}
