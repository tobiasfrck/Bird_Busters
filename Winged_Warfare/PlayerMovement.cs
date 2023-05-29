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

        public static double x = Math.PI / 2;
        public static double y = 0;
        private static Vector3 target;
        private static Vector3 Position;
        private static Vector3 change = new Vector3(0, 0, 0);

        public static float speed = 0.1f;

        public static void movement()
        {
            target = Player.GetCamTarget();
            Position = Player.GetCamPosition();
            change = new Vector3(0, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                change.Z += (Position.X - target.X);
                change.X += -(Position.Z - target.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                change.Z += -(Position.X - target.X);
                change.X += (Position.Z - target.Z);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                change.Z += -(Position.Z - target.Z);
                change.Y += -(Position.Y - target.Y);
                change.X += -(Position.X - target.X);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                change.Z += (Position.Z - target.Z);
                change.Y += (Position.Y - target.Y);
                change.X += (Position.X - target.X);
            }

            Player.SetCamPosition(Position + (change*speed));
            Player.SetCamTarget(target + (change*speed));

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Player.SetCamPosition(Player.GetCamPosition() + new Vector3(0, speed, 0));
                Player.SetCamTarget(Player.GetCamTarget() + new Vector3(0, speed, 0));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Player.SetCamPosition(Player.GetCamPosition() - new Vector3(0, speed, 0));
                Player.SetCamTarget(Player.GetCamTarget() - new Vector3(0, speed, 0));
            }

            //Dehen um Y-Achse
            target = new Vector3((float)Math.Cos(x), 0, (float)Math.Sin(x));
            Player.SetCamTarget(Player.GetCamPosition() + target);

            //Drehen um X/Z-Achse - WIP

            target = new Vector3((float)0, (float)y, (float)0);
            Player.SetCamTarget(Player.GetCamTarget() - target);
        }

        public static void setX(float n)
        {
            x += n;
        }
        public static void setY(float n)
        {

            y += n;
            //Soll verhindern, dass man sich um 180° über / unter dem Körper dreht (Im Moment eh sinnlos da das nicht möglich ist)
            if (y >= 3) y = 2.99f;
            if (y <= -3) y = -2.99f;
            //TODO: uncomment this for debug
            //Debug.WriteLine("Last Y= " + y);
        }


    }
}
