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
    public class BulletHandler
    {

        public static List<Net> bullets = new List<Net>();
        public static int reloadTime = 30;
        public static int reload = 400;
        public static bool canShoot = true;


        public static void update()
        {
            Vector3 pos = Player.CamPosition;
            Vector3 tar = Player.CamTarget;
            Debug.WriteLine(bullets.Count);


            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].position.Y <= -20)
                {
                    bullets.RemoveAt(i);
                }
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Y) && canShoot)
            {
                bullets.Add(new Net(pos, tar));
                reload = 0;
                canShoot = false;
            }
            foreach (Net net in bullets)
            {
                net.Update();
            }
            if (!canShoot) { reload += 1; }
            if (reload > reloadTime) canShoot = true;

        }
        public static void Draw()
        {
            foreach (Net net in bullets)
            {
                net.Draw();
            }
        }
        
    }
}
