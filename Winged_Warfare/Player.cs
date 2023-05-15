using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    //Does most of the stuff the player does. So:
    //Movement, has camera, has position, has bounding-box, has powerUps, has life.
    public class Player
    {
        //Camera
        public static Vector3 CamTarget;
        public static Vector3 CamPosition;
        public static Matrix ProjectionMatrix; //3D into 2D
        public static Matrix ViewMatrix; //Location and Orientation of virtual Camera?
        public static Matrix WorldMatrix; //position in Space of objects

        public static double x=Math.PI/2;
        public static double y=0;
        private static Vector3 target;
        private static Vector3 Position;
        private static Vector3 change = new Vector3(0, 0, 0);

        public static float speed = 0.1f;
        //Camera end

        public Player()
        {
            
        }

        public static void Update()
        {
            playermovement();
            ViewMatrix = Matrix.CreateLookAt(CamPosition, CamTarget, Vector3.Up);
        }

        // Camera related methods
        public static Vector3 GetCamPosition() => Player.CamPosition;
        public static Vector3 GetCamTarget() => Player.CamTarget;
        public static void SetCamTarget(Vector3 target) => Player.CamTarget = target;
        public static void SetCamPosition(Vector3 pos) => Player.CamPosition = pos;
        // Camera related methods end



        // Movement ist abhängig von der Kamera. die Bewegungen verändern ebenfalls den Punkt wo die Kamera hinschaut,
        // sodass die eigene Position bei WASD und der Blickpunkt zusammen "wandern"
        // Mouse input bewegt den Blickpunkt um den Spieler herum.

        public static void playermovement()
        {
            target = Player.GetCamTarget();
            Position = Player.GetCamPosition();
            change = new Vector3(0, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                change.Z = (Position.X - target.X) * speed;
                change.X = -(Position.Z - target.Z) * speed;

                Player.SetCamPosition(Position + change);
                Player.SetCamTarget(target + change);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                change.Z = -(Position.X - target.X) * speed;
                change.X = (Position.Z - target.Z) * speed;

                Player.SetCamPosition(Position + change);
                Player.SetCamTarget(target + change);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                change.Z = -(Position.Z - target.Z) * speed;
                change.Y = -(Position.Y - target.Y) * speed;
                change.X = -(Position.X - target.X) * speed;

                Player.SetCamPosition(Position + change);
                Player.SetCamTarget(target + change);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                change.Z = (Position.Z - target.Z) * speed;
                change.Y = (Position.Y - target.Y) * speed;
                change.X = (Position.X - target.X) * speed;

                Player.SetCamPosition(Position+change);
                Player.SetCamTarget(target+change);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Player.SetCamPosition(GetCamPosition() + new Vector3(0, speed, 0));
                Player.SetCamTarget(GetCamTarget() + new Vector3(0, speed, 0));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                Player.SetCamPosition(GetCamPosition() - new Vector3(0, speed, 0));
                Player.SetCamTarget(GetCamTarget() - new Vector3(0, speed, 0));
            }

            //Dehen um Y-Achse
                target = new Vector3((float) Math.Cos(x), 0,(float)Math.Sin(x));
                Player.SetCamTarget(Player.GetCamPosition()+target);

            //Drehen um X/Z-Achse - WIP

                target = new Vector3(0, (float)y, 0);
                Player.SetCamTarget(Player.GetCamTarget()-target);
        }

        // Nur relevant für das umschauen mit der Maus
        // Geben an die Movementfunktion weiter, um wie viel sich die Maus bewegt hat
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
            Debug.WriteLine("Last Y= " + y);
        }
    }
}
  