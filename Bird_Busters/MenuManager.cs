using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Bird_Busters
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
        private readonly SpriteBatch _spriteBatch;
        private readonly Rectangle _fullScreen = new(0, 0, Game1.Width, Game1.Height);

        private readonly Texture2D _menuBackground;
        private readonly Texture2D _settingsBackground;
        private readonly Texture2D _gameBackground;
        private readonly Texture2D _gameEndedBackground;
        private readonly Texture2D _blankTexture;
        private readonly Texture2D _menuBar;

        private readonly List<Texture2D> _menuTextures = new();
        private readonly List<Button> _menuButtons = new();
        private readonly List<Button> _settingsButtons = new();
        private readonly List<Button> _gameButtons = new();
        private readonly List<Button> _gameEndedButtons = new();
        private GameState _state = GameState.Menu; // Menu, Settings, Game, GameEnded

        private float _horizontalCenter;

        private Timer _gameEndFadeTimer;
        private Timer _gameEndAppearTimer;
        private Timer _birdAnimationTimer;
        private Timer _scoreCountingTimer;
        private Timer _birdCountingTimer;
        private Timer _animationAppearTimer;

        private readonly List<ScoreIndicator> _scoreIndicators = new();

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private bool _gameNeedsReset = false;
        private bool _isFullscreen = false;

        //HUD Layout Constants
        private const int BorderPadding = 15;
        private const float ScoreXOffset = 15;
        private readonly Vector2 _highscoreTextSize;
        private readonly Vector2 _scoreTextSize;
        private Vector2 _scoreNumberTextSize;
        private Vector2 _highscoreNumberTextSize;
        private int _remainingScoreYSpace;
        private Vector2 _timeNumberSize;
        private readonly Vector2 _timeTextSize;
        private readonly int _xOffset;
        private readonly int _yOffset;
        private readonly float _ammoX;
        private readonly int _magDisplayXPosition;
        private readonly int _magDisplayDistanceBetween; //distance between ammo icons
        private readonly Rectangle _crosshairRect;
        private readonly Vector2 _creatorCreditsSize;


        public static bool IsGunRecoiled = false;

        private bool IsCheatActivated()
        {
            // Überprüfe hier deine Cheat-Bedingungen
            if (BulletHandler.isCheatActivated()) // Hier ein Beispiel, ändere dies entsprechend deinem Cheat-Code
            {
                return true; // Cheat ist aktiviert
            }
            return false; // Cheat ist nicht aktiviert
        }

        //for testing purposes
        public static MenuManager Instance;

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
            SwitchToMenu();

            Instance = this;

            // Margin for all HUD elements on the right side of the screen
            _xOffset = Game1.Width - BorderPadding;

            _timeTextSize = Game1.HUDScoreTextFont.MeasureString("TIME");
            _highscoreTextSize = Game1.HUDScoreTextFont.MeasureString("HIGHSCORE");
            _scoreTextSize = Game1.HUDScoreTextFont.MeasureString("SCORE");
            _yOffset = Game1.Height - BorderPadding - 36;
            _magDisplayXPosition = (int)(_xOffset - Game1.HUDAmmoBG.Width / 2f - Game1.HUDAmmo.Width / 2f);
            _magDisplayDistanceBetween = -7 - Game1.HUDAmmo.Height;
            _ammoX = Game1.HUDAmmoFont.MeasureString("AMMO").X;
            _crosshairRect = new Rectangle((Game1.Width / 2) - (Game1.Crosshair.Width / 2), (Game1.Height / 2) - (Game1.Crosshair.Height / 2), Game1.Crosshair.Width, Game1.Crosshair.Height);
            _creatorCreditsSize = Game1.TestFont.MeasureString("Game Credits: Tobias Fr. | Domenik \"Steph\" B. | Roman Schmidt | Jannik Keddy");
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





                    for (int i = 0; i < _scoreIndicators.Count; i++)
                    {
                        _scoreIndicators[i].Update();
                        if (_scoreIndicators[i].AnimationDone())
                        {
                            _scoreIndicators.RemoveAt(i);
                        }
                    }

                    if (!Game1._gameTimer.IsRunning())
                    {
                        MediaPlayer.Pause();
                    }
                    else if (MediaPlayer.State == MediaState.Paused)
                    {
                        MediaPlayer.Resume();
                    }
                    break;
                case GameState.GameEnded:
                    _gameEndFadeTimer.Update();
                    _gameEndAppearTimer.Update();
                    _scoreCountingTimer?.Update();
                    _birdCountingTimer?.Update();
                    _animationAppearTimer?.Update();
                    _birdAnimationTimer?.Update();
                    if (_gameEndAppearTimer.IsRunning() == false && _scoreCountingTimer == null)
                    {
                        _scoreCountingTimer = new Timer(1000);
                    }

                    if (_scoreCountingTimer != null && _scoreCountingTimer.IsRunning() == false && _animationAppearTimer == null)
                    {
                        _animationAppearTimer = new Timer(300);
                    }


                    if (_animationAppearTimer != null && _animationAppearTimer.IsRunning() == false && _birdAnimationTimer == null)
                    {
                        _birdAnimationTimer = new Timer(1000);
                    }

                    if (_birdAnimationTimer != null && _birdAnimationTimer.GetProgress() > 0.0f && _birdCountingTimer == null)
                    {
                        _birdCountingTimer = new Timer(1500);
                    }

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

                    _spriteBatch.Draw(Game1.menuBoxLeft, new Rectangle(30, 70, 550, 700), Color.White); //linke Menu-Box
                    _spriteBatch.Draw(Game1.menuBoxRight, new Rectangle(1334, 70, 550, 700), Color.White); //rechte Menu-Box
                    _spriteBatch.Draw(Game1.menuBoxLow, new Rectangle(516, 804, 900, 240), Color.White); //untere Menu-Box
                    _spriteBatch.Draw(Game1.menuBoxCenter, new Rectangle(600, 30, 720, 756), Color.White); //mittlere Menu-Box
                    _spriteBatch.Draw(Game1.menuBoxCenter, new Rectangle(600, 30, 720, 756), Color.White); //mittlere Menu-Box
                                                                                                           //_spriteBatch.Draw(Game1.Soundbar, new Rectangle(986, 183, 234, 48), Color.White); //SoundbarMusic
                                                                                                           //_spriteBatch.Draw(Game1.Soundbar, new Rectangle(986, 252, 234, 48), Color.White); //SoundbarMusicSFX
                                                                                                           //_spriteBatch.Draw(Game1.Soundbar, new Rectangle(986, 314, 234, 48), Color.White); //SoundbarMusic
                    _spriteBatch.DrawString(Game1.TestFont, Game1.MusicVolume.ToString(), new Vector2(1060, 180), Color.White);
                    _spriteBatch.DrawString(Game1.TestFont, Game1.SFXVolume.ToString(), new Vector2(1060, 250), Color.White);
                    _spriteBatch.DrawString(Game1.TestFont, Game1.MenuSFXVolume.ToString(), new Vector2(1060, 310), Color.White);




                    string cheatStatusText = IsCheatActivated() ? "Cheat activated!" : "Cheat deactivated.";
                    Vector2 cheatStatusPosition = new(950, 555); // CameraPosition der Textausgabe
                    _spriteBatch.DrawString(Game1.TestFont, cheatStatusText, cheatStatusPosition, Color.White);

                    _spriteBatch.DrawString(Game1.TestFont, "Game Credits: Tobias Fr. | Domenik \"Steph\" B. | Roman Schmidt | Jannik Keddy", new Vector2(_horizontalCenter - _creatorCreditsSize.X / 2f, Game1.Height - _creatorCreditsSize.Y), Color.White);


                    foreach (Button btn in _settingsButtons)
                    {
                        btn.Draw(_spriteBatch, _currentMouseState.Position.ToVector2(), _currentMouseState.LeftButton == ButtonState.Pressed);
                    }
                    break;
                case GameState.Game:
                    if (Level.IsHUDDeactivated()) { break; }

                    foreach (ScoreIndicator indicator in _scoreIndicators)
                    {
                        if (!IsNotValidScreenPos(indicator.GetScreenPosition()) && !indicator.AnimationDone())
                        {
                            _spriteBatch.DrawString(Game1.HUDScoreTextFont, indicator.GetScore(), indicator.GetScreenPosition(), indicator.GetScoreColor());
                        }
                    }


                    //TODO: Clean unused code if you are sure it is not needed! So probably later.
                    // Would draw the game background here but currently not used
                    if (_gameBackground != null)
                    {
                        _spriteBatch.Draw(_gameBackground, _fullScreen, Color.White);
                    }

                    if (FPSCamera.isSprinting() == false && FPSCamera.IsScoped() == false)
                    {
                        if (IsGunRecoiled)
                        {
                            _spriteBatch.Draw(Game1.RecoiledGun, new Rectangle(0, 0, Game1.Width, Game1.Height), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(Game1.NormalGun, new Rectangle(0, 0, Game1.Width, Game1.Height), Color.White);
                        }
                    }
                    else if (FPSCamera.IsScoped() == true && FPSCamera.isSprinting() == false)
                    {
                        if (IsGunRecoiled)
                        {
                            _spriteBatch.Draw(Game1.ScopedRecoiledGun, new Rectangle(0, 0, Game1.Width, Game1.Height), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(Game1.ScopedGun, new Rectangle(0, 0, Game1.Width, Game1.Height), Color.White);
                        }
                    }

                    DrawDebug();

                    _timeNumberSize = Game1.HUDTimerFont.MeasureString(Game1._gameTimer.GetSeconds().ToString());
                    _scoreNumberTextSize = Game1.HUDScoreNumFont.MeasureString(Score.GetScore().ToString());
                    _highscoreNumberTextSize = Game1.HUDScoreNumFont.MeasureString(Score.GetHighscore().ToString());
                    _remainingScoreYSpace = (int)((Game1.HUDScoreBG.Height - _highscoreNumberTextSize.Y - _highscoreTextSize.Y) / 2f + 10);


                    //Draw HUD Backgrounds
                    _spriteBatch.Draw(Game1.HUDTimerBG, new Rectangle(BorderPadding, BorderPadding, Game1.HUDTimerBG.Width, Game1.HUDTimerBG.Height), new Color(Color.White, 0.35f));
                    _spriteBatch.Draw(Game1.HUDScoreBG, new Rectangle(_xOffset - Game1.HUDScoreBG.Width, BorderPadding, Game1.HUDScoreBG.Width, Game1.HUDScoreBG.Height), new Color(Color.White, 0.35f));
                    _spriteBatch.Draw(Game1.HUDAmmoBG, new Rectangle(_xOffset - Game1.HUDAmmoBG.Width, Game1.Height - Game1.HUDAmmoBG.Height - BorderPadding, Game1.HUDAmmoBG.Width, Game1.HUDAmmoBG.Height), new Color(Color.White, 0.35f));
                    _spriteBatch.DrawString(Game1.HUDTimerFont, Game1._gameTimer.GetSeconds().ToString(), new Vector2(BorderPadding + (Game1.HUDTimerBG.Width / 2f) - (_timeNumberSize.X / 2f), BorderPadding + (Game1.HUDTimerBG.Height / 2f) - (_timeNumberSize.Y / 2f)), Color.White);
                    _spriteBatch.DrawString(Game1.HUDScoreTextFont, "TIME", new Vector2(BorderPadding + (Game1.HUDTimerBG.Width / 2f) - (_timeTextSize.X / 2f), BorderPadding + Game1.HUDTimerBG.Height - 2), Color.White);
                    _spriteBatch.DrawString(Game1.HUDScoreTextFont, "HIGHSCORE", new Vector2(_xOffset - _highscoreTextSize.X - ScoreXOffset, BorderPadding + Game1.HUDScoreBG.Height - _remainingScoreYSpace - _highscoreTextSize.Y), new Color(207, 167, 46));
                    _spriteBatch.DrawString(Game1.HUDScoreTextFont, "SCORE", new Vector2(_xOffset - Game1.HUDScoreBG.Width + ScoreXOffset, BorderPadding + Game1.HUDScoreBG.Height - _remainingScoreYSpace - _scoreTextSize.Y), new Color(180, 76, 75));
                    _spriteBatch.DrawString(Game1.HUDScoreNumFont, Score.GetScore().ToString(), new Vector2(_xOffset - Game1.HUDScoreBG.Width + ScoreXOffset + _scoreTextSize.X / 2f - _scoreNumberTextSize.X / 2f, BorderPadding + _remainingScoreYSpace), Color.White);
                    _spriteBatch.DrawString(Game1.HUDScoreNumFont, Score.GetHighscore().ToString(), new Vector2(_xOffset - _highscoreNumberTextSize.X / 2f - _highscoreTextSize.X / 2f - ScoreXOffset, BorderPadding + _remainingScoreYSpace), Color.White);

                    // Draw Ammunition HUD
                    for (int i = 0; i < BulletHandler.GetMagazinSize(); i++)
                    {
                        int magDisplayYPosition = (i + 1) * _magDisplayDistanceBetween + (_yOffset + 1);
                        //Draw unavailable bullets while reloading
                        if (BulletHandler.IsReloading() == true)
                        {
                            _spriteBatch.Draw(Game1.HUDAmmoReloading, new Rectangle(_magDisplayXPosition, magDisplayYPosition, Game1.HUDAmmoReloading.Width, Game1.HUDAmmoReloading.Height), Color.White);
                        }
                        //Draw available bullets
                        else if (i < BulletHandler.GetAvailableShots() && BulletHandler.IsReloading() == false)
                        {
                            _spriteBatch.Draw(Game1.HUDAmmo, new Rectangle(_magDisplayXPosition, magDisplayYPosition, Game1.HUDAmmo.Width, Game1.HUDAmmo.Height), Color.White);
                        }
                        else //Draw unavailable bullets
                        {
                            _spriteBatch.Draw(Game1.HUDAmmoEmpty, new Rectangle(_magDisplayXPosition, magDisplayYPosition, Game1.HUDAmmoEmpty.Width, Game1.HUDAmmoEmpty.Height), Color.White);
                        }
                    }
                    _spriteBatch.DrawString(Game1.HUDAmmoFont, "AMMO", new Vector2((_xOffset - Game1.HUDAmmoBG.Width / 2f - _ammoX / 2f), _yOffset - 3), Color.White);


                    //Draw crosshair
                    _spriteBatch.Draw(Game1.Crosshair, _crosshairRect, Color.White);
                    break;
                case GameState.GameEnded:
                    if (_gameEndedBackground != null)
                    {
                        _spriteBatch.Draw(_gameEndedBackground, _fullScreen, new Color(Color.Transparent, _gameEndFadeTimer.GetProgress()));
                    }

                    if (_gameEndAppearTimer.IsRunning())
                    {
                        break;
                    }

                    Vector2 gameEndedDim = Game1.HUDTimerFont.MeasureString("Game Ended");
                    Vector2 scoreDim = Game1.HUDScoreTextFont.MeasureString("Score: " + Score.GetScore());
                    _spriteBatch.DrawString(Game1.HUDTimerFont, "Game Ended", new Vector2(_horizontalCenter - gameEndedDim.X / 2, 25), Color.White);
                    _spriteBatch.DrawString(Game1.HUDScoreTextFont, "Score: " + (int)(Score.GetScore() * _scoreCountingTimer.GetProgress()), new Vector2(_horizontalCenter - scoreDim.X / 2, 130), Color.White);

                    if (_scoreCountingTimer != null && _scoreCountingTimer.IsRunning() == false)
                    {
                        Vector2 highscoreDim = Game1.HUDScoreTextFont.MeasureString("Highscore: " + Score.GetHighscore());
                        if (Score.IsNewHighscore())
                        {
                            highscoreDim = Game1.HUDScoreTextFont.MeasureString("New Highscore!");
                            _spriteBatch.DrawString(Game1.HUDScoreTextFont, "New Highscore!", new Vector2(_horizontalCenter - highscoreDim.X / 2, 180), Color.White);
                        }
                        else
                        {
                            _spriteBatch.DrawString(Game1.HUDScoreTextFont, "Highscore: " + Score.GetHighscore(), new Vector2(_horizontalCenter - highscoreDim.X / 2, 180), Color.White);
                        }
                    }

                    if (_birdAnimationTimer != null)
                    {
                        int frame = (int)(_birdAnimationTimer.GetProgress() * 60);
                        if (_birdAnimationTimer.IsRunning() == false)
                        {
                            frame = 0;
                        }
                        _spriteBatch.Draw(Game1.GreenBirdVideo[frame], new Rectangle(30, 200, Game1.GreenBirdVideo[frame].Width / 2, Game1.GreenBirdVideo[frame].Height / 2 + 100), Color.White);
                        _spriteBatch.Draw(Game1.RedBirdVideo[frame], new Rectangle(716, 200, Game1.RedBirdVideo[frame].Width / 2, Game1.RedBirdVideo[frame].Height / 2 + 100), Color.White);
                        _spriteBatch.Draw(Game1.OrangeBirdVideo[frame], new Rectangle(1400, 200, Game1.OrangeBirdVideo[frame].Width / 2, Game1.OrangeBirdVideo[frame].Height / 2 + 100), Color.White);

                    }

                    if (_birdCountingTimer != null)
                    {
                        float measureGX = Game1.HUDScoreNumFont.MeasureString(((int)(Score.GetBirdsHit(BirdType.Common) * _birdCountingTimer.GetProgress())).ToString()).X;
                        float measureRX = Game1.HUDScoreNumFont.MeasureString(((int)(Score.GetBirdsHit(BirdType.Rare) * _birdCountingTimer.GetProgress())).ToString()).X;
                        float measureLX = Game1.HUDScoreNumFont.MeasureString(((int)(Score.GetBirdsHit(BirdType.Legendary) * _birdCountingTimer.GetProgress())).ToString()).X;

                        _spriteBatch.DrawString(Game1.HUDScoreNumFont, ((int)(Score.GetBirdsHit(BirdType.Common) * _birdCountingTimer.GetProgress())).ToString(), new Vector2(30 + Game1.GreenBirdVideo[0].Width / 4f - measureGX / 2f, 400 + Game1.GreenBirdVideo[0].Height / 4f), Color.White);
                        _spriteBatch.DrawString(Game1.HUDScoreNumFont, ((int)(Score.GetBirdsHit(BirdType.Rare) * _birdCountingTimer.GetProgress())).ToString(), new Vector2(716 + Game1.RedBirdVideo[0].Width / 4f - measureRX / 2, 400 + Game1.RedBirdVideo[0].Height / 4f), Color.White);
                        _spriteBatch.DrawString(Game1.HUDScoreNumFont, ((int)(Score.GetBirdsHit(BirdType.Legendary) * _birdCountingTimer.GetProgress())).ToString(), new Vector2(1400 + Game1.OrangeBirdVideo[0].Width / 4f - measureLX / 2, 400 + Game1.OrangeBirdVideo[0].Height / 4f), Color.White);
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

        [Conditional("DEBUG")]
        private void DrawDebug()
        {
            if (Level.GetDebugMode())
            {
                Vector2 textDim = Game1.TestFont.MeasureString(Level.GetDebugText());
                _spriteBatch.DrawString(Game1.TestFont, Level.GetDebugText(),
                    new Vector2((Game1.Width / 2f) - (textDim.X / 2f), Game1.Height - textDim.Y), Color.Red);
            }
        }

        private void CreateMainMenu()
        {
            float XFractionStart = 1f / 2f;
            float YFractionStart = 6f / 10f;

            float XFractionSettings = 4f / 10f;
            float YFractionSettings = 8f / 10f;

            float XFractionExit = 1 - XFractionSettings;
            float YFractionExit = 8f / 10f;

            Button startGame = new(new Vector2(Game1.Width * XFractionStart, Game1.Height * YFractionStart), new Vector2(250, 125), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Start Game", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button settings = new(new Vector2(Game1.Width * XFractionSettings, Game1.Height * YFractionSettings), new Vector2(250, 125), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Settings", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button exitGame = new(new Vector2(Game1.Width * XFractionExit, Game1.Height * YFractionExit), new Vector2(250, 125), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Exit", Game1.TestFont, Color.Black, Color.Black, Color.Black);

            _menuButtons.Add(startGame);
            _menuButtons.Add(settings);
            _menuButtons.Add(exitGame);
            startGame.SetClick(SwitchToGame); //action for button 0
            settings.SetClick(SwitchToSettings); //action for button 1
            exitGame.SetClick(CloseGame);
        }

        private void CreateSettingsMenu()
        {
            float XFraction = 1f / 18f;
            float YFraction = 9f / 10f;

            Button toMenu = new(new Vector2(Game1.Width * XFraction, Game1.Height * YFraction), new Vector2(100, 60), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Back", Game1.TestFont, Color.Black, Color.White, Color.White);
            _settingsButtons.Add(toMenu);
            toMenu.SetClick(SwitchToMenu);

            /*
             // Erstelle den "UHD" Button (3840x2160)
             Button UHD = new Button(new Vector2(Game1.Width * 0.63f, Game1.Height * 0.41f), new Vector2(130, 40), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "UHD", Game1.TestFont, Color.Black, Color.White, Color.White);

             _settingsButtons.Add(UHD);
             UHD.SetClick(SetResolutionToUHD);

             void SetResolutionToUHD()
             {
                 _graphics.PreferredBackBufferWidth = 3840;
                 _graphics.PreferredBackBufferHeight = 2160;
                 _graphics.ApplyChanges();
             }

             // Erstelle den "Full HD" Button (1920x1080)
             Button fullHD = new Button(new Vector2(Game1.Width * 0.53f, Game1.Height * 0.41f), new Vector2(130, 40), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Full-HD", Game1.TestFont, Color.Black, Color.White, Color.White);

             _settingsButtons.Add(fullHD);
             fullHD.SetClick(SetResolutionToFullHD);

             void SetResolutionToFullHD()
             {
                 _graphics.PreferredBackBufferWidth = 1920;
                 _graphics.PreferredBackBufferHeight = 1080;
                 _graphics.ApplyChanges();
             }

             // Erstelle den "HD" Button(1280x720)
             Button HDbutton = new Button(new Vector2(Game1.Width * 0.53f, Game1.Height * 0.47f), new Vector2(130, 40), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "HD", Game1.TestFont, Color.Black, Color.White, Color.White);

             _settingsButtons.Add(HDbutton);
             HDbutton.SetClick(SetResolutionToHD);

             void SetResolutionToHD()
             {
                 _graphics.PreferredBackBufferWidth = 1280;
                 _graphics.PreferredBackBufferHeight = 720;
                 _graphics.ApplyChanges();
             }

             // Erstelle den "HD-Plus" Button (1600x900)
             Button HDplus = new Button(new Vector2(Game1.Width * 0.63f, Game1.Height * 0.47f), new Vector2(130, 40), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "HD+", Game1.TestFont, Color.Black, Color.White, Color.White);

             _settingsButtons.Add(HDplus);
             HDplus.SetClick(SetResolutionToHDplus);

             void SetResolutionToHDplus()
             {
                 _graphics.PreferredBackBufferWidth = 1600;
                 _graphics.PreferredBackBufferHeight = 900;
                 _graphics.ApplyChanges();
             }
            */
            // Button, um in den Vollbildmodus zu wechseln
            Button fullscreenButton = new(new Vector2(Game1.Width * 0.544f, Game1.Height * 0.43f), new Vector2(175, 50), Game1.Button, Game1.ButtonHover, Game1.ButtonPressed, "Fullscreen", Game1.TestFont, Color.Black, Color.White, Color.White);
            fullscreenButton.SetClick(ToggleFullscreen);
            _settingsButtons.Add(fullscreenButton);

            // Methode zum Wechseln zwischen Vollbild- und Fenstermodus
            void ToggleFullscreen()
            {
                _isFullscreen = !_isFullscreen; // Zustand umschalten

                if (_isFullscreen)
                {
                    _graphics.IsFullScreen = true;

                    fullscreenButton.SetText("Window"); // Ändere den Text auf "Fenstermodus"
                }
                else
                {
                    _graphics.IsFullScreen = false;
                    fullscreenButton.SetText("Fullscreen"); // Ändere den Text auf "Vollbild"
                }

                // Wende die Änderungen an
                _graphics.ApplyChanges();
            }

            void IncreaseVolume()
            {
                // Erhöhung der Lautstärke
                Game1.MusicVolume += 10;
                if (Game1.MusicVolume > 100)
                {
                    Game1.MusicVolume = 100;
                }
                MediaPlayer.Volume = Game1.MusicVolume / 100f;
            }

            void DecreaseVolume()
            {
                // Reduzieren der Lautstärke
                Game1.MusicVolume -= 10;

                // fällt nicht unter 0
                if (Game1.MusicVolume < 0)
                {
                    Game1.MusicVolume = 0;
                }
                MediaPlayer.Volume = Game1.MusicVolume / 100f;
            }

            // Gesamt-Lautstärke erhöhen Button 
            float volumeUpButtonX = Game1.Width * 0.65f;
            float volumeUpButtonY = Game1.Height * 0.17f;
            Button volumeUpButton = new(
                new Vector2(volumeUpButtonX, volumeUpButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "+", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            volumeUpButton.SetClick(IncreaseVolume);
            _settingsButtons.Add(volumeUpButton);

            // Gesamt-Lautstärke reduzieren Button
            float volumeDownButtonX = Game1.Width * 0.5f;
            float volumeDownButtonY = Game1.Height * 0.17f;
            Button volumeDownButton = new(
                new Vector2(volumeDownButtonX, volumeDownButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "-", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            volumeDownButton.SetClick(DecreaseVolume);
            _settingsButtons.Add(volumeDownButton);


            void IncreaseSFXVolume()
            {
                if (Game1.SFXVolume < 100) // Überprüfen, ob die maximale Lautstärke nicht überschritten wird
                {
                    Game1.SFXVolume += 10; // Erhöhe die SFX-Lautstärke um 10
                }
            }

            void DecreaseSFXVolume()
            {
                // Reduzieren der Lautstärke
                Game1.SFXVolume -= 10;

                // fällt nicht unter 0
                if (Game1.SFXVolume < 0)
                {
                    Game1.SFXVolume = 0;
                }
            }

            // Soundeffekt-Lautstärke erhöhen Button
            float svolumeUpButtonX = Game1.Width * 0.65f;
            float svolumeUpButtonY = Game1.Height * 0.233f;
            Button svolumeUpButton = new(
                new Vector2(svolumeUpButtonX, svolumeUpButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "+", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            svolumeUpButton.SetClick(IncreaseSFXVolume);
            _settingsButtons.Add(svolumeUpButton);

            // Soundeffekt-Lautstärke reduzieren Button
            float svolumeDownButtonX = Game1.Width * 0.5f;
            float svolumeDownButtonY = Game1.Height * 0.233f;
            Button svolumeDownButton = new(
                new Vector2(svolumeDownButtonX, svolumeDownButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "-", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            svolumeDownButton.SetClick(DecreaseSFXVolume);
            _settingsButtons.Add(svolumeDownButton);

            void IncreaseMenuSFXVolume()
            {
                if (Game1.MenuSFXVolume < 100) // Überprüfen, ob die maximale Lautstärke nicht überschritten wird
                {
                    Game1.MenuSFXVolume += 10; // Erhöhe die MenuSFX-Lautstärke um 10
                }
            }

            void DecreaseMenuSFXVolume()
            {
                // Reduzieren der MenuSFX-Lautstärke
                Game1.MenuSFXVolume -= 10;

                // fällt nicht unter 0
                if (Game1.MenuSFXVolume < 0)
                {
                    Game1.MenuSFXVolume = 0;
                }
            }

            // MenuSFX-Lautstärke erhöhen Button
            float sMvolumeUpButtonX = Game1.Width * 0.65f;
            float sMvolumeUpButtonY = Game1.Height * 0.29f;
            Button sMvolumeUpButton = new(
                new Vector2(sMvolumeUpButtonX, sMvolumeUpButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "+", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            sMvolumeUpButton.SetClick(IncreaseMenuSFXVolume);
            _settingsButtons.Add(sMvolumeUpButton);

            // MenuSFX-Lautstärke reduzieren Button
            float sMvolumeDownButtonX = Game1.Width * 0.5f;
            float sMvolumeDownButtonY = Game1.Height * 0.29f;
            Button sMvolumeDownButton = new(
                new Vector2(sMvolumeDownButtonX, sMvolumeDownButtonY),
                new Vector2(40, 40),
                Game1.Button, Game1.ButtonHover, Game1.ButtonPressed,
                "-", Game1.TestFont, Color.Black, Color.Black, Color.Black
            );
            sMvolumeDownButton.SetClick(DecreaseMenuSFXVolume);
            _settingsButtons.Add(sMvolumeDownButton);
        }

        private void CreateGameEndedMenu()
        {
            Vector2 backSize = Game1.TestFont.MeasureString("Return to menu");
            Vector2 restartSize = Game1.TestFont.MeasureString("Play another round");
            Vector2 backPadding = new(40, 40);
            Vector2 restartPadding = new(40, 40);

            Button toMenu = new(new Vector2(270 + backPadding.X, (Game1.Height - 100)) - backPadding, backSize + backPadding, Game1.Grey80, Game1.Grey40, _blankTexture, "Return to menu", Game1.TestFont, Color.Black, Color.Black, Color.Black);
            Button restart = new(new Vector2(1650 + restartPadding.X, (Game1.Height - 100)) - restartPadding, restartSize + restartPadding, Game1.Grey80, Game1.Grey40, _blankTexture, "Play another round", Game1.TestFont, Color.Black, Color.Black, Color.Black);
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
            if (_state != GameState.Settings)
            {
                MediaPlayer.Volume = Game1.MusicVolume / 100f;
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Game1.startScreenMusic);
            }

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
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(Game1.gameScreenMusic);
            MediaPlayer.Volume = Game1.MusicVolume / 100f;
            Game1._gameTimer = new Timer((int)Game1.gameScreenMusic.Duration.TotalMilliseconds, SetGameNeedsReset);
            //uncomment for testing endscreen
            //Game1._gameTimer = new Timer(10000, SetGameNeedsReset);
            Mouse.SetPosition(Game1.Width / 2, Game1.Height / 2);
            Button.ResetConflicts();
            SetState(GameState.Game);
            Debug.WriteLine("Switched to game state.");
            Game1.Instance.ResetGame();
        }

        public void SwitchToEndscreen()
        {
            Game1.EndOfRoundSoundEffect.Play();
            _gameEndFadeTimer = new Timer(1236);
            _gameEndAppearTimer = new Timer(1660);
            _scoreCountingTimer = null;
            _animationAppearTimer = null;
            _birdAnimationTimer = null;
            _birdCountingTimer = null;
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

        public void AddScoreIndicator(Vector3 position, String score, BirdType birdType)
        {
            _scoreIndicators.Add(new ScoreIndicator(position, score, birdType));
        }

        private bool IsNotValidScreenPos(Vector2 pos)
        {
            if (pos.X.Equals(-1) || pos.Y.Equals(-1))
            {
                //Debug.WriteLine("Invalid screen position");
                return true;
            }
            return false;
        }

    }
}
