﻿using System;
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

        //TODO: Delete testing variables
        public Model TestCube;
        private int _cubePos=-100;
        //---------------------------

        //dictionary to look up models by name
        public static Dictionary<string, Model> Models = new();
        
        //contains all model names
        private static readonly string[] _modelNames = {
            "testContent/testCube"
        };

        //Level
        private Level _level;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //IsFixedTimeStep = false; // relevant for frameRate; maybe later
            IsMouseVisible = false;
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1920; // you can change this if its too big or too small for your screen
            _graphics.PreferredBackBufferHeight = 1080; // you can change this if its too big or too small for your screen
            //_graphics.ApplyChanges(); // apply changes to the graphics device manager; not needed here but if you change the screen size during runtime you need this
        }

        private void LoadModels()
        {
            //create dictionary with all models
            foreach (var modelName in _modelNames)
            {
                Models.Add(modelName, Content.Load<Model>(modelName));
            }
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


            Debug.WriteLine("Game initialized in Game1.cs.");
            MouseMovement.Init();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            TestCube = Content.Load<Model>("testContent/testCube");

            LoadModels();
            _level = new Level();
            _level.LoadLevel("Levels/sampleLevel.txt");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            _level.UpdateObjects();
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
             Matrix world = Matrix.CreateScale(0.1f);
             world *= Matrix.CreateRotationX(MathHelper.ToRadians(90));
             world *= Matrix.CreateTranslation(new Vector3(0, 2, -90));
            TestCube.Draw(world,Player.ViewMatrix,Player.ProjectionMatrix);
            //_cubePos += 1;
            //Debug.WriteLineIf(_cubePos>150,_cubePos);
            
            //Debug.WriteLine("FPS: " + 1000/gameTime.ElapsedGameTime.TotalMilliseconds); //Outputs FPS to console
            _level.DrawModels();
            base.Draw(gameTime);
        }


    }
}