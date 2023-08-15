using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    public class FPSCamera
    {
        // The angle of rotation about the Y-axis
        float horizontalAngle;
        // The angle of rotation about the X-axis
        float verticalAngle;
        // The camera's position in the world 
        public static Vector3 position;
        public static Vector3 StartPosition;
        // The state of the mouse in the prior frame
        MouseState oldMouseState;
        // The Game this camera belongs to 
        Game game;
        public static Vector3 direction;
        public static Vector3 lookAt;

        /// <summary>
        /// The view matrix for this camera
        /// </summary>
        public static Matrix View { get; protected set; }

        /// <summary>
        /// The projection matrix for this camera
        /// </summary>
        public static Matrix Projection { get; protected set; }

        /// <summary>
        /// The sensitivity of the mouse when aiming
        /// </summary>
        public float Sensitivity { get; set; } = 0.0009f;


        /// <summary>
        /// The speed of the player while moving 
        /// </summary>
        public float Speed { get; set; } = 0.009f;

        //Gravity, Flight & Velocity
        static bool creativeFlight = false;
        static bool flightButtonPressed = false;
        static bool IsGrounded;
        static Vector3 _change;
        static float Velocity;
        static float Gravity = -0.001f;
        public static bool IsSprinting;
        public static bool IsMoving;
        static float POVadjusted;

        private const int StepCooldown = 50; // Zeitabstand zwischen zwei Schrittsounds (in MilliSekunden)
        private Timer _stepTimer = new(StepCooldown);
        private SoundEffect[] _stepSounds = Game1.StepSounds;

        //Audio Listener
        public AudioListener Listener = new AudioListener();

        //Collision Corners
        public Vector2 Corner1 = new Vector2(-2.9f, -2.4f);
        public Vector2 Corner2 = new Vector2(-0.1f, 2.4f);

        //Standart Settings
        float POV = MathHelper.ToRadians(45);

        /// <summary>
        /// Constructs a new FPS Camera
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The player's initial position</param>
        public FPSCamera(Game _game, Vector3 _position)
        {
            this.game = _game;
            position = _position;
            StartPosition = position;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), game.GraphicsDevice.DisplayMode.AspectRatio, 0.001f, 1000);
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();
            POVadjusted = POV;
            Listener = new AudioListener();
            Listener.Position = position;
        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public void Update(GameTime gameTime)
        {
            _stepTimer.Update();
            var keyboard = Keyboard.GetState();
            var newMouseState = Mouse.GetState();

            float Speed = 0.03f;
            IsMoving = false;
            IsSprinting = false;

            //Sprinting
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                Speed += Speed / 2;
                IsSprinting = true;
            }

            // Get the direction the player is currently facing
            var facing = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(horizontalAngle));
            // Forward and backward movement
            if (keyboard.IsKeyDown(Keys.W)) { position += facing * Speed; IsMoving = true; }
            if (keyboard.IsKeyDown(Keys.S)) { position -= facing * Speed; IsMoving = true; }
            // Strifing movement
            if (keyboard.IsKeyDown(Keys.A)) { position += Vector3.Cross(Vector3.Up, facing) * Speed; IsMoving = true; }
            if (keyboard.IsKeyDown(Keys.D)) { position -= Vector3.Cross(Vector3.Up, facing) * Speed; IsMoving = true; }
            // Adjust horizontal angle
            horizontalAngle += Sensitivity * (oldMouseState.X - newMouseState.X);

            // SFX: steps while on ground
            if (IsMoving && IsGrounded && _stepTimer.RestartIfTimeElapsed())
            {
                int soundIndex = Game1.RandomGenerator.Next(Game1.StepSounds.Length);
                _stepTimer.SetTimeNRun((int)Math.Round(_stepSounds[soundIndex].Duration.TotalMilliseconds + StepCooldown,0,MidpointRounding.AwayFromZero));
                _stepSounds[soundIndex].Play(Game1.SFXVolume * 0.075f, 0, 0);
            }

            // Adjust vertical angle 
            verticalAngle += Sensitivity * (oldMouseState.Y - newMouseState.Y);
            if (verticalAngle <= -1.56f) verticalAngle = -1.559f;
            if (verticalAngle >= 1.56f) verticalAngle = 1.559f;
            direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(verticalAngle) * Matrix.CreateRotationY(horizontalAngle));
            // Debug.WriteLine("verticalAngle= " + verticalAngle);




            //Smooth POV when starting to sprint/stoping to sprint
            if (!IsSprinting || !IsMoving && POVadjusted > POV)
            {
                POVadjusted = POVadjusted * 0.99f;
            }

            if (IsSprinting && IsMoving && POVadjusted <= POV * 1.1f)
            {
                POVadjusted = POVadjusted * 1.01f;
            }
            if (POVadjusted < POV) POVadjusted = POV;
            if (POVadjusted > POV * 1.1f) POVadjusted = POV * 1.1f;

            //Change POV when the Player is zooming (Right Mouse Button)
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !(IsSprinting && IsMoving))
            {
                POVadjusted = POV * 0.5f;
            }


            //Jumping
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsGrounded == true && !creativeFlight)
            {
                Velocity = 0.025f;
                IsGrounded = false;
            }

            //Creative Flight
            _change = new Vector3(0, 0, 0);
            CheckFlight();
            if (creativeFlight)
            {
                IsGrounded = false;
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _change.Y += 1 * Speed;
                }

                if (keyboard.IsKeyDown(Keys.LeftControl))
                {
                    _change.Y -= 1 * Speed;
                }
            }
            else
            {

                position.Y += Velocity;
                Velocity += Gravity;

                if (position.Y <= 0.2f)
                {
                    // SFX: landing
                    if (!IsGrounded && _stepTimer.RestartIfTimeElapsed())
                    {
                        int soundIndex = Game1.RandomGenerator.Next(Game1.StepSounds.Length);
                        _stepTimer.SetTimeNRun((int)Math.Round(_stepSounds[soundIndex].Duration.TotalMilliseconds + StepCooldown, 0, MidpointRounding.AwayFromZero));
                        _stepSounds[soundIndex].Play(Game1.SFXVolume * 0.075f, 0, 0);
                    }

                    position.Y = 0.2f;
                    IsGrounded = true;
                }
            }
            position += _change;

            //Corner Collision
            if (!creativeFlight)
            {
                if (position.X < Corner1.X) position.X = Corner1.X;
                if (position.Z < Corner1.Y) position.Z = Corner1.Y;
                if (position.X > Corner2.X) position.X = Corner2.X;
                if (position.Z > Corner2.Y) position.Z = Corner2.Y;
            }

            // recreate the ViewMatrix
            View = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
            // recreate ProjectionMatrix
            Projection = Matrix.CreatePerspectiveFieldOfView(POVadjusted, game.GraphicsDevice.Viewport.AspectRatio, 0.001f, 1000);
            // Reset mouse state 
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();

            Listener.Position = position;
            Listener.Forward = direction;
        }

        private static void CheckFlight()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.F2) && flightButtonPressed)
                flightButtonPressed = false;
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && !flightButtonPressed)
            {
                creativeFlight = !creativeFlight;
                flightButtonPressed = true;
            }
        }
        public void Reset()
        {
            position = StartPosition;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
        }
    }
}
