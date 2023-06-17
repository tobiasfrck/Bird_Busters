using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
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
        public static bool Exit = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int Width = 1920;
        public static int Height = 1080;
        private bool _wasUnfocused = false;

        //TODO: Delete testing variables
        public Model TestCube;
        public Model planeModel;
        public Model NetModel;
        private int _cubePos = -100;
        Matrix rotationmatrix;
        Vector3 rotatedVector;
        float angle = 0;
        private float radius = 10;
        //---------------------------

        //Testing for bird
        private Bird _bird;

        // The game camera
        FPSCamera camera;

        //Assets
        //dictionary to look up models by name
        public static Dictionary<string, Model> Models = new();

        //contains all model names
        private static readonly string[] _modelNames = {
            "testContent/testCube",
            "testContent/planeTest",
            "testContent/Net",
            "Level_Concept"
        };
        public static SpriteFont TestFont;
        public static Texture2D Button;
        public static Texture2D ButtonHover;
        public static Texture2D ButtonPressed;
        //Assets end

        //Level
        private Level _level;

        //Menu
        private MenuManager _menuManager;

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
        }



        protected override void Initialize()
        {
            base.Initialize();
            // TODO: Add your initialization logic here
            // Initialize camera in Game1.cs because of "No Graphics Device Service" problem.
            Player.CamPosition = new Vector3(-1f, 0.2f, -1f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z + 1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.01f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end

            //TODO: Replace textures with actual textures
            _menuManager = new MenuManager(_spriteBatch, null, Content.Load<Texture2D>("testContent/button"), null, null);
            // Initialize the camera 
            camera = new FPSCamera(this, Player.CamPosition);

            Debug.WriteLine("Game initialized in Game1.cs.");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            TestCube = Content.Load<Model>("testContent/testCube");
            NetModel = Content.Load<Model>("testContent/Net");
            TestFont = Content.Load<SpriteFont>("testContent/testFont");
            Button = Content.Load<Texture2D>("testContent/button");
            ButtonHover = Content.Load<Texture2D>("testContent/button_hover");
            ButtonPressed = Content.Load<Texture2D>("testContent/button_pressed");



            //TODO: Load textures for _menuManager

            LoadModels();
            _level = new Level();
            _level.LoadLevel("Levels/sampleLevel.txt");
        }

        private void LoadModels()
        {
            //create dictionary with all models
            foreach (var modelName in _modelNames)
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
                    _level.UpdateObjects();
                    Player.Update();
                    BulletHandler.Update();
                    BirdHandler.Update();
                    // Update the camera
                    camera.Update(gameTime);
                    break;
                case GameState.GameEnded:
                    break;
                default:
                    Debug.WriteLine("[Error]: Menu state not found.");
                    break;
            }


            //If Player has won or lost
            if (Score.hasWon())
            {
                _menuManager.SwitchToMenu(); //this needs to be changed to a win/lose screen
                Debug.WriteLine("--------");
                Debug.WriteLine("You won!");
                Debug.WriteLine("--------");
                Score.ResetScore();
            }


            //Used to end the game with other classes
            if (Exit)
            {
                Exit();
            }

            //Testing may be deleted
            Vector3 save = new Vector3(Player.CamTarget.X, Player.CamTarget.Y, Player.CamTarget.Z);
            save.Normalize();
            rotationmatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            rotatedVector = Vector3.Transform(Player.CamPosition, rotationmatrix);
            angle += 1f;
            float x = (float)Math.Sin(MathHelper.ToRadians(angle)) * radius;
            float z = (float)Math.Cos(MathHelper.ToRadians(angle)) * radius;
            rotatedVector = Player.CamPosition + new Vector3(x, 0, z);
            //Testing end



            //Tests related to birds


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

            //Debug.WriteLine("[FPS]: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console

            //READ THIS!
            //DO NOT DRAW ANYTHING RELATED TO THE ACTUAL GAME OUTSIDE OF THE IF STATEMENT
            if (_menuManager.GetState() == GameState.Game) //everything that should be drawn in game state
            {
                //This fixed broken Models with SpriteBatch and 3D Models. 
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                
                //Testing and may be deleted
                Matrix world = Matrix.CreateScale(0.1f);
                world *= Matrix.CreateRotationX(MathHelper.ToRadians(90));
                world *= Matrix.CreateTranslation(new Vector3(0, 2, 0));
                TestCube.Draw(world, Player.ViewMatrix, Player.ProjectionMatrix);
                TestCube.Draw(Matrix.CreateTranslation(rotatedVector), Player.ViewMatrix, Player.ProjectionMatrix);
                //Testing end

                _level.DrawModels();
                BulletHandler.Draw();
                BirdHandler.Draw();
            }

            

            _spriteBatch.Begin();
            _menuManager.Draw();
            _spriteBatch.End();
            //Do not draw 3D models after this.

            base.Draw(gameTime);
        }



    }
}