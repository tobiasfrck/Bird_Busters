using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{
    //Switches between game states:
    //States: in menu, in settings, in game, game ended
    public class Game1 : Game
    {
        public static bool exit = false;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int Width = 1920;
        public static int Height = 1080;

        //TODO: Delete testing variables
        public Model TestCube;
        public Model planeModel;
        private int _cubePos = -100;
        Matrix rotationmatrix;
        Vector3 rotatedVector;
        float angle = 0;
        private float radius = 10;
        //---------------------------

        //Testing for bird
        private Bird _bird;


        //Assets
        //dictionary to look up models by name
        public static Dictionary<string, Model> Models = new();

        //contains all model names
        private static readonly string[] _modelNames = {
            "testContent/testCube",
            "testContent/planeTest",
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
            Player.CamPosition = new Vector3(0f, 0f, 0f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z + 1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 0.01f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end


            Debug.WriteLine("Game initialized in Game1.cs.");

            MouseMovement.Init();
            

            //TODO: Replace textures with actual textures
            _menuManager = new MenuManager(_spriteBatch, Content.Load<Texture2D>("testContent/button"), Content.Load<Texture2D>("testContent/button"), Content.Load<Texture2D>("testContent/button"), Content.Load<Texture2D>("testContent/button"));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            TestCube = Content.Load<Model>("testContent/testCube");

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _menuManager.SwitchToMenu();

            // TODO: Add your update logic here

            _menuManager.Update();
            switch (_menuManager.GetState()) // 0 = menu, 1 = settings, 2 = game, 3 = game ended
            {
                case 0: //menu state
                    IsMouseVisible = true;
                    break;
                case 1: //settings state
                    IsMouseVisible = true;
                    break;
                case 2: //game state
                    IsMouseVisible = false;
                    _level.UpdateObjects();
                    Player.Update();
                    MouseMovement.Update();
                    break;
                case 3: //game ended state
                    IsMouseVisible = true;
                    break;
                default:
                    IsMouseVisible = true;
                    Debug.WriteLine("Error: Menu state not found.");
                    break;
            }

            if (exit)
            {
                Exit();
            }

            //test
            Vector3 save = new Vector3(Player.CamTarget.X, Player.CamTarget.Y, Player.CamTarget.Z);
            save.Normalize();
            rotationmatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
            rotatedVector = Vector3.Transform(Player.CamPosition, rotationmatrix);
            angle += 1f;
            float x = (float)Math.Sin(MathHelper.ToRadians(angle)) * radius;
            float z = (float)Math.Cos(MathHelper.ToRadians(angle)) * radius;
            rotatedVector = Player.CamPosition + new Vector3(x, 0, z);

            BulletHandler.update();
            

            //Tests related to birds


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Delete testing code
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Uncomment to draw invisible faces of a model. Useful for debugging.
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rasterizerState;

            //Debug.WriteLine("FPS: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console

            if (_menuManager.GetState() == 2) //everything that should be drawn in game state
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default; //This fixed broken Models with SpriteBatch and 3D Models
                Matrix world = Matrix.CreateScale(0.1f);
                world *= Matrix.CreateRotationX(MathHelper.ToRadians(90));
                world *= Matrix.CreateTranslation(new Vector3(0, 2, 0));
                TestCube.Draw(world, Player.ViewMatrix, Player.ProjectionMatrix);
                TestCube.Draw(Matrix.CreateTranslation(rotatedVector), Player.ViewMatrix, Player.ProjectionMatrix);

                _level.DrawModels();
            }

            BulletHandler.Draw();

            _spriteBatch.Begin();
            _menuManager.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }



    }
}