using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;

namespace Bird_Busters
{
    public class FPSCamera
    {
        // The angle of rotation about the Y-axis
        private float horizontalAngle;
        // The angle of rotation about the X-axis
        float verticalAngle;
        // The camera's position in the world 
        public static Vector3 Position;
        public static Vector3 StartPosition;
        // The state of the mouse in the prior frame
        MouseState oldMouseState;
        // The Game this camera belongs to 
        private readonly Game _game;
        public static Vector3 direction;
        public static bool isScoping = false;

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
        private static float MaxSpeed { get; set; } = 0.13f;

        private static Vector2 _speedVector = new(0, 0);

        private const float _moveGainAccel = 0.019f;
        private const float _moveLossAccel = 0.014f;

        //Gravity, Flight & Velocity
        static bool creativeFlight = false;
        static bool flightButtonPressed = false;
        static bool IsGrounded;
        static Vector3 _change;
        static float Velocity;
        private const float Gravity = -0.0025f;
        public static bool IsSprinting;
        private static bool _wasSprinting;
        public static bool IsMoving;
        static float POVadjusted;

        private const int StepCooldown = 50; // Zeitabstand zwischen zwei Schrittsounds (in MilliSekunden)
        private readonly Timer _stepTimer = new(StepCooldown);
        private readonly SoundEffect[] _stepSounds = Game1.StepSounds;

        //Audio Listener
        public readonly AudioListener Listener;

        //Collision Corners
        private readonly Vector2 _corner1 = new(-2.9f, -2.4f);
        private readonly Vector2 _corner2 = new(12.128665f, 16.450724f);

        //Standart Settings
        private static readonly float Pov = MathHelper.ToRadians(70);

        /// <summary>
        /// Constructs a new FPS Camera
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The player's initial position</param>
        public FPSCamera(Game game, Vector3 position)
        {
            this._game = game;
            Position = position;
            StartPosition = Position;
            this.horizontalAngle = MathHelper.ToRadians(90f);
            this.verticalAngle = 0;
            Projection = Matrix.CreatePerspectiveFieldOfView(Pov, this._game.GraphicsDevice.DisplayMode.AspectRatio, 0.001f, 1000);
            Mouse.SetPosition(this._game.Window.ClientBounds.Width / 2, this._game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();
            POVadjusted = Pov;
            Listener = new AudioListener
            {
                Position = Position
            };
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
            //Debug.WriteLine("Speed:" + Speed);

            IsMoving = false;

            //Sprinting
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                IsSprinting = true;
            }
            else
            {
                if (IsSprinting)
                {
                    _wasSprinting = true;
                }
                IsSprinting = false;
            }

            ;

            // Get the direction the player is currently facing
            var facing = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(horizontalAngle + MathHelper.ToRadians(180f)));
            // Forward and backward movement
            if (keyboard.IsKeyDown(Keys.W))
            {
                if (IsSprinting)
                {
                    _speedVector.X += _moveGainAccel * 1.5f;
                }
                else
                {
                    _speedVector.X += _moveGainAccel;
                }
                IsMoving = true;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                if (IsSprinting)
                {
                    _speedVector.X -= _moveGainAccel * 1.5f;
                }
                else
                {
                    _speedVector.X -= _moveGainAccel;
                }
                IsMoving = true;
            }

            // Strifing movement
            if (keyboard.IsKeyDown(Keys.A))
            {
                if (IsSprinting)
                {
                    _speedVector.Y += _moveGainAccel * 1.5f;
                }
                else
                {
                    _speedVector.Y += _moveGainAccel;
                }
                IsMoving = true;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                if (IsSprinting)
                {
                    _speedVector.Y -= _moveGainAccel * 1.5f;
                }
                else
                {
                    _speedVector.Y -= _moveGainAccel;
                }
                IsMoving = true;
            }




            // Adjust horizontal angle
            horizontalAngle += Sensitivity * (oldMouseState.X - newMouseState.X);

            if (IsMoving && IsSprinting)
            {
                if (_speedVector.X > MaxSpeed * 1.5f)
                {
                    _speedVector.X = MaxSpeed * 1.5f;
                }

                if (_speedVector.Y > MaxSpeed * 1.5f)
                {
                    _speedVector.Y = MaxSpeed * 1.5f;
                }

                if (_speedVector.X < MaxSpeed * -1.5f)
                {
                    _speedVector.X = MaxSpeed * -1.5f;
                }

                if (_speedVector.Y < MaxSpeed * -1.5f)
                {
                    _speedVector.Y = MaxSpeed * -1.5f;
                }

                LimitSpeed(1.3f);

            }
            if (IsMoving && !IsSprinting && !_wasSprinting) // Is moving but not sprinting and was not sprinting before
            {
                if (_speedVector.X > MaxSpeed)
                {
                    _speedVector.X = MaxSpeed;
                }

                if (_speedVector.Y > MaxSpeed)
                {
                    _speedVector.Y = MaxSpeed;
                }

                if (_speedVector.X < MaxSpeed * -1)
                {
                    _speedVector.X = MaxSpeed * -1;
                }

                if (_speedVector.Y < MaxSpeed * -1)
                {
                    _speedVector.Y = MaxSpeed * -1;
                }

                LimitSpeed(1f);
            }
            if (IsMoving && _wasSprinting)
            {
                if (_speedVector.X > MaxSpeed)
                {
                    _speedVector.X = MaxSpeed;
                }
                else if (_speedVector.X < MaxSpeed * -1)
                {
                    _speedVector.X = MaxSpeed * -1;
                }

                if (_speedVector.Y > MaxSpeed)
                {
                    _speedVector.Y = MaxSpeed;
                }
                else if (_speedVector.Y < MaxSpeed * -1)
                {
                    _speedVector.Y = MaxSpeed * -1;
                }


                if ((_speedVector.X < MaxSpeed && _speedVector.X > 0 && _speedVector.Y < MaxSpeed && _speedVector.Y > 0) || (_speedVector.X > MaxSpeed * -1 && _speedVector.X < 0 && _speedVector.Y > MaxSpeed * -1 && _speedVector.Y < 0))
                {
                    _wasSprinting = false;
                }

                LimitSpeed(1f);
            }
            bool xSlowDown = false;
            bool ySlowDown = false;
            bool xSpeedUp = false;
            bool ySpeedUp = false;
            if (keyboard.IsKeyUp(Keys.A) && keyboard.IsKeyUp(Keys.D)) // Is not moving
            {
                if (_speedVector.Y > 0)
                {
                    _speedVector.Y -= _moveLossAccel;
                    ySlowDown = true;
                }

                if (_speedVector.Y < 0)
                {
                    _speedVector.Y += _moveLossAccel;
                    ySpeedUp = true;
                }

                if (ySlowDown && ySpeedUp)
                {
                    _speedVector.Y = 0;
                }
            }

            if (keyboard.IsKeyUp(Keys.W) && keyboard.IsKeyUp(Keys.S))
            {
                if (_speedVector.X > 0)
                {
                    _speedVector.X -= _moveLossAccel;
                    xSlowDown = true;
                }
                if (_speedVector.X < 0)
                {
                    _speedVector.X += _moveLossAccel;
                    xSpeedUp = true;
                }
                if (xSlowDown && xSpeedUp)
                {
                    _speedVector.X = 0;
                }
            }

            if (ySlowDown && ySpeedUp && xSlowDown && xSpeedUp)
            {
                IsMoving = false;
            }

            if (_speedVector.X > 0)
            {
                Position += facing * _speedVector.X;
            }

            if (_speedVector.X < 0)
            {
                Position -= facing * -1 * _speedVector.X;
            }

            if (_speedVector.Y > 0)
            {
                Position += Vector3.Cross(Vector3.Up, facing) * _speedVector.Y;
            }

            if (_speedVector.Y < 0)
            {
                Position -= Vector3.Cross(Vector3.Up, facing) * -1 * _speedVector.Y;
            }

            // SFX: steps while on ground
            if (IsMoving && IsGrounded && _stepTimer.RestartIfTimeElapsed())
            {
                int soundIndex = Game1.RandomGenerator.Next(Game1.StepSounds.Length);
                _stepTimer.SetTimeNRun((int)Math.Round(_stepSounds[soundIndex].Duration.TotalMilliseconds + StepCooldown, 0, MidpointRounding.AwayFromZero));
                _stepSounds[soundIndex].Play(Game1.SFXVolume / 100f * 0.25f, 0, 0);
            }

            // Adjust vertical angle 
            verticalAngle += Sensitivity * (oldMouseState.Y - newMouseState.Y);
            if (verticalAngle <= -1.56f) verticalAngle = -1.559f;
            if (verticalAngle >= 1.56f) verticalAngle = 1.559f;
            direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(verticalAngle) * Matrix.CreateRotationY(horizontalAngle + MathHelper.ToRadians(180f)));
            // Debug.WriteLine("verticalAngle= " + verticalAngle);


            //Smooth POV when starting to sprint/stoping to sprint
            if (!IsSprinting || !IsMoving && POVadjusted > Pov)
            {
                POVadjusted = POVadjusted * 0.99f;
            }

            if (IsSprinting && IsMoving && POVadjusted <= Pov * 1.1f)
            {
                POVadjusted = POVadjusted * 1.01f;
            }


            if (POVadjusted < Pov) POVadjusted = Pov;
            if (POVadjusted > Pov * 1.1f) POVadjusted = Pov * 1.1f;

            //Change POV when the Player is zooming (Right Mouse Button)
            if (Mouse.GetState().RightButton == ButtonState.Pressed && !(IsSprinting && IsMoving))
            {
                POVadjusted = Pov * 0.5f;
                isScoping = true;
            }
            else
            {
                isScoping = false;
            }


            //Jumping
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsGrounded == true && !creativeFlight)
            {
                Velocity = 0.050f;
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
                    _change.Y += 1 * 0.18f;
                }

                if (keyboard.IsKeyDown(Keys.LeftControl))
                {
                    _change.Y -= 1 * 0.18f;
                }
            }
            else
            {

                Position.Y += Velocity;
                Velocity += Gravity;

                if (Position.Y <= 0.2f)
                {
                    // SFX: landing
                    if (!IsGrounded && _stepTimer.RestartIfTimeElapsed())
                    {
                        int soundIndex = Game1.RandomGenerator.Next(Game1.StepSounds.Length);
                        _stepTimer.SetTimeNRun((int)Math.Round(_stepSounds[soundIndex].Duration.TotalMilliseconds + StepCooldown, 0, MidpointRounding.AwayFromZero));
                        _stepSounds[soundIndex].Play(Game1.SFXVolume / 100f * 0.25f, 0, 0);
                    }

                    Position.Y = 0.2f;
                    IsGrounded = true;
                }
            }
            Position += _change;

            //Corner Collision
            if (!creativeFlight)
            {
                if (Position.X < _corner1.X) Position.X = _corner1.X;
                if (Position.Z < _corner1.Y) Position.Z = _corner1.Y;
                if (Position.X > _corner2.X) Position.X = _corner2.X;
                if (Position.Z > _corner2.Y) Position.Z = _corner2.Y;
            }

            // recreate the ViewMatrix
            View = Matrix.CreateLookAt(Position, Position + direction, Vector3.Up);
            // recreate ProjectionMatrix
            Projection = Matrix.CreatePerspectiveFieldOfView(POVadjusted, _game.GraphicsDevice.Viewport.AspectRatio, 0.001f, 1000);
            // Reset mouse state 
            Mouse.SetPosition(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
            oldMouseState = Mouse.GetState();

            Listener.Position = Position;
            Listener.Forward = direction;
        }

        public static bool IsScoped()
        {
            return isScoping;
        }

        public static bool isSprinting()
        {
            return IsSprinting;
        }

        private void LimitSpeed(float factor)
        {
            if (_speedVector.X * _speedVector.X + _speedVector.Y * _speedVector.Y > MaxSpeed * factor)
            {
                if (_speedVector.X > 0)
                {
                    _speedVector.X = (float)Math.Sqrt(MaxSpeed * factor);
                }
                else
                {
                    _speedVector.X = (float)Math.Sqrt(MaxSpeed * factor) * -1;
                }

                if (_speedVector.Y > 0)
                {
                    _speedVector.Y = (float)Math.Sqrt(MaxSpeed * factor);
                }
                else
                {
                    _speedVector.Y = (float)Math.Sqrt(MaxSpeed * factor) * -1;
                }
            }
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
            Position = StartPosition;
            this.horizontalAngle = 0;
            this.verticalAngle = 0;
        }
    }
}
