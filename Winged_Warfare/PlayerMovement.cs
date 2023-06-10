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
    internal class PlayerMovement
    {

        public static double X = Math.PI / 2;
        public static double Y = 0;
        private static Vector3 _target;
        private static Vector3 _position;
        private static Vector3 _change = new Vector3(0, 0, 0);

        public static float Gravity = -0.0019f;
        public static float Velocity = 0f;
        public static bool IsGrounded = false;

        public static void Movement()
        {

            float speed = 0.01f;
            _target = Player.GetCamTarget();
            _position = Player.GetCamPosition();
            _change = new Vector3(0, 0, 0);


            if (Keyboard.GetState().IsKeyDown(Keys.Space) && IsGrounded == true)
            {
                Velocity = 0.025f;
                IsGrounded = false;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _change.Z += (_position.X - _target.X);
                _change.X += -(_position.Z - _target.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _change.Z += -(_position.X - _target.X);
                _change.X += (_position.Z - _target.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _change.Z += -(_position.Z - _target.Z);
                //change.Y += -(Position.Y - target.Y);
                _change.X += -(_position.X - _target.X);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _change.Z += (_position.Z - _target.Z);
                //change.Y += (Position.Y - target.Y);
                _change.X += (_position.X - _target.X);
            }



            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //{
            //     Player.SetCamPosition(Player.GetCamPosition() + new Vector3(0, speed, 0));
            //    Player.SetCamTarget(Player.GetCamTarget() + new Vector3(0, speed, 0));
            //}

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Player.SetCamPosition(Player.GetCamPosition() - new Vector3(0, speed, 0));
                Player.SetCamTarget(Player.GetCamTarget() - new Vector3(0, speed, 0));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                speed += speed;
            }



            _position.Y += Velocity;
            Velocity += Gravity;

            if (_position.Y <= 0.2f)
            {
                _position.Y = 0.2f;
                IsGrounded = true;
            }

            Player.SetCamPosition(_position + (_change * speed));
            Player.SetCamTarget(_target + (_change * speed));


            //Drehen um Y-Achse
            _target = new Vector3((float)Math.Cos(X), 0, (float)Math.Sin(X));
            Player.SetCamTarget(Player.GetCamPosition() + _target);

            //Drehen um X/Z-Achse - WIP
            _target = new Vector3((float)0, (float)Y, (float)0);
            Player.SetCamTarget(Player.GetCamTarget() - _target);
        }

        public static void SetX(float n)
        {
            X += n;
        }

        public static void SetY(float n)
        {

            Y += n;
            //Soll verhindern, dass man sich um 180° über / unter dem Körper dreht (Im Moment eh sinnlos da das nicht möglich ist)
            if (Y >= 3) Y = 2.99f;
            if (Y <= -3) Y = -2.99f;
            //TODO: uncomment this for debug
            //Debug.WriteLine("Last Y= " + y);
        }


    }
}
