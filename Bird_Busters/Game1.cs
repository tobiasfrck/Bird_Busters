﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Bird_Busters
{
    //Switches between game states:
    //States: in menu, in settings, in game, game ended
    public class Game1 : Game
    {
        private const int Seed = 0; // Seed for random generator; Seed 0 is for random seed; This is for testing purposes.
        public static Random RandomGenerator;
        public static bool CloseGame = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool _wasUnfocused = false;
        public static float Fps = 0;
        private bool fullscreenbutton = false;

        // Game Settings
        public static int Width = 1920;
        public static int Height = 1080;
        public static int MusicVolume = 10;
        public static int SFXVolume = 100;
        public static int MenuSFXVolume = 10;

        public bool isMusicPlaying = true;







        //TODO: Delete testing variables
        //---------------------------

        // The game camera
        private FPSCamera _camera;

        //Assets
        //dictionary to look up models by name
        public static readonly Dictionary<string, Model> Models = new();

        //contains all model names
        private static readonly string[] ModelNames = {
            "testContent/testCube",
            "testContent/Net",
            "Level/City_Level",
            "Shaders/skyBoxCube",
            "8Ball_Net",
            "Net_Low_Poly",
            "Birds/Birb",
            "Birds/Birb2",
            "Birds/Birb3"
        };
        public static SpriteFont TestFont;
        public static SpriteFont TestFont48;
        public static SpriteFont HUDTimerFont;
        public static SpriteFont HUDScoreTextFont;
        public static SpriteFont HUDScoreNumFont;
        public static SpriteFont HUDAmmoFont;


        public static Texture2D menuBackground;
        public static Texture2D settingsBackground;
        public static Texture2D gameBackground;
        public static Texture2D menuBoxLeft;
        public static Texture2D menuBoxRight;
        public static Texture2D menuBoxLow;
        public static Texture2D menuBoxCenter;
        public static Texture2D Soundbar;



        public static Texture2D Button;
        public static Texture2D ButtonHover;
        public static Texture2D ButtonPressed;
        public static Texture2D Grey40;
        public static Texture2D Grey80;

        public static Texture2D NormalGun;
        public static Texture2D RecoiledGun;
        public static Texture2D ScopedGun;
        public static Texture2D ScopedRecoiledGun;
        public static Texture2D HUDTimerBG;
        public static Texture2D HUDAmmoBG;
        public static Texture2D HUDScoreBG;
        public static Texture2D HUDAmmo;
        public static Texture2D HUDAmmoEmpty;
        public static Texture2D HUDAmmoReloading;
        public static Texture2D Crosshair;
        public static Texture2D[] GreenBirdVideo;
        public static Texture2D[] RedBirdVideo;
        public static Texture2D[] OrangeBirdVideo;


        public static SoundEffect BtnHoverSfx;
        public static SoundEffect BtnClickSfx;
        public static SoundEffect ShootEffect;
        public static SoundEffect HitMarker;
        public static SoundEffect Reload_Mag;

        public static SoundEffect[] BirdFlaps;

        public static SoundEffect[] StepSounds;

        public static SoundEffect EndOfRoundSoundEffect;
        public static Song startScreenMusic;
        public static Song gameScreenMusic;
        //Assets end

        //Audio
        public static AudioListener Listener;


        //Level
        private Level _level;

        //Menu
        private static MenuManager _menuManager;

        //Game
        public static Game1 Instance;

        //Gameloop Timer
        public static Timer _gameTimer;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //IsFixedTimeStep = false; // relevant for frameRate; maybe later
            IsMouseVisible = true;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = Width; // you can change this if its too big or too small for your screen
            _graphics.PreferredBackBufferHeight = Height; // you can change this if its too big or too small for your screen
            //_graphics.ApplyChanges(); // apply changes to the graphics device manager; not needed here but if you change the screen size during runtime you need this
            Instance = this;

            if (Seed == 0) // If seed is 0, use random seed
            {
                RandomGenerator = new Random();
            }
            else
            {
                RandomGenerator = new Random(Seed);
            }
        }



        protected override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization logic here
            // Initialize camera in Game1.cs because of "No _graphics Device Service" problem.
            Player.CamPosition = new Vector3(3.94f, 0.2f, 7.71f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z + 1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.01f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end

            //TODO: Replace textures with actual textures
            _menuManager = new MenuManager(_graphics, _spriteBatch, menuBackground, settingsBackground, gameBackground, Grey40);
            BulletHandler._menuManager = _menuManager;
            BulletHandler.CreateDeletionZoneList();

            // Initialize the camera 
            _camera = new FPSCamera(this, Player.CamPosition);
            Listener = _camera.Listener;

            //BirdHandler.CreateList();
            //BirdHandler._startPoints = _level.GetStartPoints();

            Debug.WriteLine("Game initialized in Game1.cs.");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Fonts
            TestFont = Content.Load<SpriteFont>("Font_SS3_Bold_24");
            HUDTimerFont = Content.Load<SpriteFont>("Font_SS3_Bold_48");
            HUDScoreTextFont = Content.Load<SpriteFont>("Font_SS3_Bold_ScoreText");
            HUDScoreNumFont = Content.Load<SpriteFont>("Font_SS3_Bold_ScoreNumber");
            HUDAmmoFont = Content.Load<SpriteFont>("Font_SS3_Bold_AmmoText");

            // Textures
            menuBackground = Content.Load<Texture2D>("testContent/menuBackground");
            settingsBackground = Content.Load<Texture2D>("testContent/settingsBackground");
            menuBoxLeft = Content.Load<Texture2D>("testContent/TutorialBox");
            menuBoxRight = Content.Load<Texture2D>("testContent/ControlBox");
            menuBoxLow = Content.Load<Texture2D>("testContent/CreditBox");
            menuBoxCenter = Content.Load<Texture2D>("testContent/OptionsBox");
            Soundbar = Content.Load<Texture2D>("testContent/SoundAnzeige");


            //gameBackground = Content.Load<Texture2D>("testContent/gameBackground");

            Button = Content.Load<Texture2D>("testContent/button");
            ButtonHover = Content.Load<Texture2D>("testContent/button_hover");
            ButtonPressed = Content.Load<Texture2D>("testContent/button_pressed");
            Grey40 = Content.Load<Texture2D>("Grey40");
            Grey80 = Content.Load<Texture2D>("Grey80");


            // Textures for HUD
            NormalGun = Content.Load<Texture2D>("HUD_BG/Gun_Normal");
            RecoiledGun = Content.Load<Texture2D>("HUD_BG/Gun_Recoil");
            ScopedGun = Content.Load<Texture2D>("HUD_BG/Gun_Scoped");
            ScopedRecoiledGun = Content.Load<Texture2D>("HUD_BG/Gun_Scoped_Recoil");
            HUDTimerBG = Content.Load<Texture2D>("HUD_BG/Time_HUD");
            HUDAmmoBG = Content.Load<Texture2D>("HUD_BG/Ammo_HUD");
            HUDScoreBG = Content.Load<Texture2D>("HUD_BG/Score_HUD");
            HUDAmmo = Content.Load<Texture2D>("Bullet");
            HUDAmmoEmpty = Content.Load<Texture2D>("Bullet_G");
            HUDAmmoReloading = Content.Load<Texture2D>("Bullet_B");
            Crosshair = Content.Load<Texture2D>("crosshair");
            GreenBirdVideo = new Texture2D[60];
            for (int i = 1; i < 61; i++)
            {
                GreenBirdVideo[i - 1] = Content.Load<Texture2D>("BirdAnimations/Green/" + i.ToString().PadLeft(4, '0'));
            }
            RedBirdVideo = new Texture2D[60];
            for (int i = 1; i < 61; i++)
            {
                RedBirdVideo[i - 1] = Content.Load<Texture2D>("BirdAnimations/Red/" + i.ToString().PadLeft(4, '0'));
            }
            OrangeBirdVideo = new Texture2D[60];
            for (int i = 1; i < 61; i++)
            {
                OrangeBirdVideo[i - 1] = Content.Load<Texture2D>("BirdAnimations/Orange/" + i.ToString().PadLeft(4, '0'));
            }


            // SFX
            BtnHoverSfx = Content.Load<SoundEffect>("Audio/Hover_Button"); //TODO: Replace with actual sound
            BtnClickSfx = Content.Load<SoundEffect>("Audio/Click_Button"); //TODO: Replace with actual sound
            ShootEffect = Content.Load<SoundEffect>("Audio/shot");
            Reload_Mag = Content.Load<SoundEffect>("Audio/Reload");
            HitMarker = Content.Load<SoundEffect>("Audio/hitmarker");
            EndOfRoundSoundEffect = Content.Load<SoundEffect>("Audio/end_of_round");

            BirdFlaps = new SoundEffect[4];
            for (int i = 0; i < BirdFlaps.Length; i++)
            {
                BirdFlaps[i] = Content.Load<SoundEffect>("Audio/BirdFlaps/Flap_" + (i + 1) + "_60143");
            }

            StepSounds = new SoundEffect[4];
            for (int i = 0; i < StepSounds.Length; i++)
            {
                StepSounds[i] = Content.Load<SoundEffect>("Audio/stepSFX/steps_" + (i + 1));
            }


            // Music
            startScreenMusic = Content.Load<Song>("Audio/menu_music");
            gameScreenMusic = Content.Load<Song>("Audio/ingame_music");



            LoadModels();
            _level = new Level();
            _level.LoadLevel("Levels/sampleLevel.txt");
        }

        private void LoadModels()
        {
            //create dictionary with all models
            foreach (var modelName in ModelNames)
            {
                Models.Add(modelName, Content.Load<Model>(modelName));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            //Disable update if window is not focused
            if (!IsActive)
            {
                if (!_wasUnfocused)
                {
                    IsMouseVisible = true;
                    _wasUnfocused = true;
                    Debug.WriteLine("[LostFocus]: " + Mouse.GetState().Position);
                    MediaPlayer.Pause();
                }
                return;
            }

            //TODO: Reposition mouse if window was unfocused to eliminate weird movement; currently not working
            if (_wasUnfocused)
            {
                _wasUnfocused = false;
                MediaPlayer.Resume();
                Debug.WriteLine("[GotFocus]: " + Mouse.GetState().Position);
                IsMouseVisible = false; //https://github.com/MonoGame/MonoGame/issues/7842#issuecomment-1191712378
                Mouse.SetPosition(Width / 2, Height / 2);
                return;
            }

            //Escape to go back to menu, TODO: pause menu?
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Escape)) && _menuManager.GetState() != GameState.Menu)
            {
                _menuManager.SwitchToMenu();
            }


            _menuManager.Update();


            //Hide mouse if in game
            IsMouseVisible = _menuManager.GetState() != GameState.Game;

            switch (_menuManager.GetState()) // 0 = menu, 1 = settings, 2 = game, 3 = game ended
            {
                case GameState.Menu:
                    break;
                case GameState.Settings:
                    break;
                case GameState.Game:
                    _gameTimer?.Update();
                    _level.UpdateObjects();
                    Player.Update();
                    BulletHandler.Update();
                    _camera.Update(gameTime);
                    break;
                case GameState.GameEnded:
                    break;
                default:
                    Debug.WriteLine("[Error]: Menu state not found.");
                    break;
            }


            //If Game has ended, restart level
            if (_menuManager.NeedsReset())
            {
                _menuManager.SwitchToEndscreen(); //TODO: this needs to be changed to a win/lose screen
                Debug.WriteLine("--------");
                Debug.WriteLine("You won!");
                Debug.WriteLine("--------");
            }

            if (Keyboard.GetState().IsKeyUp(Keys.F11) && fullscreenbutton)
            {
                fullscreenbutton = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && !fullscreenbutton)
            {
                if (_graphics.IsFullScreen == true)
                {
                    _graphics.IsFullScreen = false;
                    Window.IsBorderless = false;
                }
                else
                {
                    _graphics.IsFullScreen = true;
                }
                fullscreenbutton = true;
                _graphics.ApplyChanges();
            }




            //Used to end the game with other classes
            if (CloseGame)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        //READ COMMENTS IN THIS METHOD
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Delete testing code
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Uncomment to draw invisible faces of a model. Useful for debugging.
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            RasterizerState rasterizerState = new();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;

            Fps = (float)(1000 / gameTime.ElapsedGameTime.TotalMilliseconds); //calculates FPS
            Timer.Fps = Fps;
            //Debug.WriteLine("[FPS]: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console

            //READ THIS!
            //DO NOT DRAW ANYTHING RELATED TO THE ACTUAL GAME OUTSIDE OF THE IF STATEMENT
            if (_menuManager.GetState() == GameState.Game || _menuManager.GetState() == GameState.GameEnded) //everything that should be drawn in game state
            {
                //This fixed broken Models with SpriteBatch and 3D Models. 
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                _level.DrawModels();
                BulletHandler.Draw();
                //BirdHandler.Draw();
            }



            _spriteBatch.Begin();
            _menuManager.Draw();
            _spriteBatch.End();
            //Do not draw 3D models after this.

            base.Draw(gameTime);
        }

        public void RestartLevel()
        {
            _level = new Level();
            _level.LoadLevel("Levels/sampleLevel.txt");
        }

        public void ResetGame()
        {
            Score.ResetScore();
            BirdSpawnpoint.Reset();
            BulletHandler.Reset();
            RestartLevel();
            if (Seed != 0)
            {
                RandomGenerator = new Random(Seed);
            }
            _camera.Reset();
        }
    }

}