using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
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
        public Matrix Projection { get; protected set; }

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
        static float Gravity= -0.001f;

        /// <summary>
        /// Constructs a new FPS Camera
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The player's initial position</param>
        public FPSCamera(Game _game, Vector3 _position)
        {
            this.game = _game;
            position = _position;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 1000);
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();


        }

        /// <summary>
        /// Updates the camera
        /// </summary>
        /// <param name="gameTime">The current GameTime</param>
        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var newMouseState = Mouse.GetState();

            float Speed = 0.03f;
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                Speed += Speed / 2;
            }



            // Get the direction the player is currently facing
            var facing = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(horizontalAngle));
            // Forward and backward movement
            if (keyboard.IsKeyDown(Keys.W)) position += facing * Speed;
            if (keyboard.IsKeyDown(Keys.S)) position -= facing * Speed;
            // Strifing movement
            if (keyboard.IsKeyDown(Keys.A)) position += Vector3.Cross(Vector3.Up, facing) * Speed;
            if (keyboard.IsKeyDown(Keys.D)) position -= Vector3.Cross(Vector3.Up, facing) * Speed;
            // Adjust horizontal angle
            horizontalAngle += Sensitivity * (oldMouseState.X - newMouseState.X);

            // Adjust vertical angle 
            verticalAngle += Sensitivity * (oldMouseState.Y - newMouseState.Y);
            if (verticalAngle <= -1.56f) verticalAngle = -1.559f;
            if (verticalAngle >= 1.56f) verticalAngle = 1.559f;
            direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(verticalAngle) * Matrix.CreateRotationY(horizontalAngle));
            Debug.WriteLine("verticalAngle= " + verticalAngle);


            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsGrounded == true && !creativeFlight)
            {
                Velocity = 0.025f;
                IsGrounded = false;
            }


            _change = new Vector3(0, 0, 0);
            checkFlight();
            if (creativeFlight)
            {
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    _change.Y += 1*Speed;
                }

                if (keyboard.IsKeyDown(Keys.LeftControl))
                {
                    _change.Y -= 1*Speed;
                }
            }
            else
            {
                position.Y += Velocity;
                Velocity += Gravity;

                if (position.Y <= 0.2f)
                {
                    position.Y = 0.2f;
                    IsGrounded = true;
                }
            }
            position += _change;

            // create the veiw matrix
            View = Matrix.CreateLookAt(position, position + direction, Vector3.Up);
            // Reset mouse state 
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();
        }

        public static void checkFlight()
        {
            if (Keyboard.GetState().IsKeyUp(Keys.F2) && flightButtonPressed)
                flightButtonPressed = false;
            if (Keyboard.GetState().IsKeyDown(Keys.F2) && !flightButtonPressed)
            {
                creativeFlight = !creativeFlight;
                flightButtonPressed = true;
            }
        }

    }
}
