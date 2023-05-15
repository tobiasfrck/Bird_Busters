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

        //TODO: Delete testing variables
        private Model _testCube;
        private int _cubePos=-100;
        //---------------------------
 


        //Level
        private Level _level;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Initialize camera in Game1.cs because of "No Graphics Device Service" problem.
            Player.CamPosition = new Vector3(0f, 2f, -100f);
            Player.CamTarget = new Vector3(Player.CamPosition.X, Player.CamPosition.Y, Player.CamPosition.Z +1);
            Player.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            Player.ViewMatrix = Matrix.CreateLookAt(Player.CamPosition, Player.CamTarget, Vector3.Up);
            Debug.WriteLine("Camera initialized in Camera.cs.");
            // Initialize camera end

            _level = new Level();

            Debug.WriteLine("Game initialized in Game1.cs.");
            MouseMovement.Init();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            _testCube = Content.Load<Model>("testContent/testCube");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here


            Player.Update();
            MouseMovement.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            // TODO: Delete testing code
            _testCube.Draw(Matrix.CreateTranslation(new Vector3(0,2,-90)),Player.ViewMatrix,Player.ProjectionMatrix);
            //_cubePos += 1;
            //Debug.WriteLineIf(_cubePos>150,_cubePos);


            base.Draw(gameTime);
        }


    }
}