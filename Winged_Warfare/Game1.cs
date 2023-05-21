using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    //Switches between game states:
    //States: in menu, in settings, in game, game ended
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int Width  = 1920;
        public static int Height = 1080;


        //TODO: Delete testing variables
        public Model TestCube;
        private int _cubePos = -100;
        //---------------------------


        //Assets
        //dictionary to look up models by name
        public static Dictionary<string, Model> Models = new();

        //contains all model names
        private static readonly string[] _modelNames = {
            "testContent/testCube"
        };
        public static SpriteFont TestFont;
        public static Texture2D ButtonPlaceholder;
        public static Texture2D ButtonHoverPlaceholder;
        public static Texture2D ButtonPressedPlaceholder;
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
            Player.CamPosition = new Vector3(0f, 2f, -100f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z + 1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end


            Debug.WriteLine("Game initialized in Game1.cs.");

            MouseMovement.Init();

            //TODO: Replace textures with actual textures
            _menuManager = new MenuManager(_spriteBatch, Content.Load<Texture2D>("testContent/placeholder1"), Content.Load<Texture2D>("testContent/placeholder1"), Content.Load<Texture2D>("testContent/placeholder1"), Content.Load<Texture2D>("testContent/placeholder1"));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            TestCube = Content.Load<Model>("testContent/testCube");

            TestFont = Content.Load<SpriteFont>("testContent/testFont");
            ButtonPlaceholder = Content.Load<Texture2D>("testContent/placeholder1");
            ButtonHoverPlaceholder = Content.Load<Texture2D>("testContent/placeholder1_hover");
            ButtonPressedPlaceholder = Content.Load<Texture2D>("testContent/placeholder1_pressed");

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
                Exit();

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
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            // TODO: Delete testing code
            
            //_cubePos += 1;
            //Debug.WriteLineIf(_cubePos>150,_cubePos);

            //Debug.WriteLine("FPS: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console

            if (_menuManager.GetState() == 2) //everything that should be drawn in game state
            {
                Matrix world = Matrix.CreateScale(0.1f);
                world *= Matrix.CreateRotationX(MathHelper.ToRadians(90));
                world *= Matrix.CreateTranslation(new Vector3(0, 2, -90));
                TestCube.Draw(world, Player.ViewMatrix, Player.ProjectionMatrix);
                _level.DrawModels();
            }
            else //everything that should be drawn in menu state
            {
                _spriteBatch.Begin();
                _menuManager.Draw();
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }


    }
}