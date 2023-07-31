using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{
    //Switches between game states:
    //States: in menu, in settings, in game, game ended
    public class Game1 : Game
    {

        public static Random RandomGenerator = new Random();
        public static bool CloseGame = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool _wasUnfocused = false;
        public static float Fps = 0;
        
        // Game Settings
        public static int Width = 1920;
        public static int Height = 1080;
        public static float MusicVolume = 0.1f;
        public static float SFXVolume = 1;
        public static float MenuSFXVolume = 0.1f;

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
            "testContent/planeTest",
            "testContent/Net",
            "Level_Concept",
            "8Ball_Net"
        };
        public static SpriteFont TestFont;

        public static Texture2D Button;
        public static Texture2D ButtonHover;
        public static Texture2D ButtonPressed;

        public static Texture2D HUDAmmo;
        public static Texture2D HUDAmmoEmpty;
        public static Texture2D HUDAmmoReloading;

        public static SoundEffect BtnHoverSfx;
        public static SoundEffect BtnClickSfx;
        public static SoundEffect ShootEffect;
        public static SoundEffect HitMarker;
        public static SoundEffect BirdFlaps;

        public static SoundEffect[] StepSounds;
        public static SoundEffect StepSound;

        public static Song gameOverMusic;
        public static Song startScreenMusic;
        public static Song gameScreenMusic;
        public static Song currentBackgroundMusic;
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
        }


        
        protected override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization logic here
            // Initialize camera in Game1.cs because of "No _graphics Device Service" problem.
            Player.CamPosition = new Vector3(-1f, 0.2f, -1f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z + 1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.01f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end

            //TODO: Replace textures with actual textures
            _menuManager = new MenuManager(_graphics,_spriteBatch, null, Content.Load<Texture2D>("testContent/button"), null, null);
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
            TestFont = Content.Load<SpriteFont>("testContent/testFont");


            // Textures
            Button = Content.Load<Texture2D>("testContent/button");
            ButtonHover = Content.Load<Texture2D>("testContent/button_hover");
            ButtonPressed = Content.Load<Texture2D>("testContent/button_pressed");

            // Textures for HUD
            HUDAmmo = Content.Load<Texture2D>("Net_HUD_Texture");
            HUDAmmoEmpty = Content.Load<Texture2D>("Net_HUD_Texture_transparent");
            HUDAmmoReloading = Content.Load<Texture2D>("Net_HUD_Texture_reloading");


            // SFX
            BtnHoverSfx = Content.Load<SoundEffect>("Audio/Hover_Button"); //TODO: Replace with actual sound
            BtnClickSfx = Content.Load<SoundEffect>("Audio/Click_Button"); //TODO: Replace with actual sound
            ShootEffect = Content.Load<SoundEffect>("Audio/shot");
            HitMarker = Content.Load<SoundEffect>("Audio/hitmarker");
            BirdFlaps = Content.Load<SoundEffect>("Audio/BirdFlaps1");
            StepSounds = new SoundEffect[4];
            for (int i = 0; i < StepSounds.Length; i++)
            {
                StepSounds[i] = Content.Load<SoundEffect>("Audio/stepSFX/steps_" + (i + 1));
            }


            // Music
            startScreenMusic = Content.Load<Song>("Audio/funk-jam");
            gameScreenMusic = Content.Load<Song>("Audio/game-jam");

            currentBackgroundMusic = startScreenMusic;
            MediaPlayer.IsRepeating = true;
            //TODO: enable music, for testing purposes disabled
            //MediaPlayer.Play(currentBackgroundMusic);
            MediaPlayer.Volume = MusicVolume;


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
                    IsMouseVisible=true; 
                    _wasUnfocused = true;
                    Debug.WriteLine("[LostFocus]: "+Mouse.GetState().Position);
                }
                return;
            }

            //TODO: Reposition mouse if window was unfocused to eliminate weird movement; currently not working
            if (_wasUnfocused)
            {
                _wasUnfocused = false;
                
                Debug.WriteLine("[GotFocus]: "+Mouse.GetState().Position);
                IsMouseVisible = false; //https://github.com/MonoGame/MonoGame/issues/7842#issuecomment-1191712378
                Mouse.SetPosition(Width/2, Height/2);
                return;
            }


            //Escape to go back to menu, TODO: pause menu?
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _menuManager.SwitchToMenu();


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
                    //BirdHandler.Update();
                    // Update the camera
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
                _menuManager.SwitchToMenu(); //this needs to be changed to a win/lose screen
                Debug.WriteLine("--------");
                Debug.WriteLine("You won!");
                Debug.WriteLine("--------");
                Score.ResetScore();
                BirdSpawnpoint.Reset();
                BulletHandler.Reset();
                RestartLevel();
                _camera.Reset();
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
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            Fps = (float)(1000 / gameTime.ElapsedGameTime.TotalMilliseconds); //calculates FPS
            Timer.Fps = Fps;
            //Debug.WriteLine("[FPS]: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console

            //READ THIS!
            //DO NOT DRAW ANYTHING RELATED TO THE ACTUAL GAME OUTSIDE OF THE IF STATEMENT
            if (_menuManager.GetState() == GameState.Game) //everything that should be drawn in game state
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
    }

}