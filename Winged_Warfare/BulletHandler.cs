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
        //Liste mit allen Bullets
        public static List<Net> bullets = new List<Net>();
        //Delay-Zeit zwischen Jedem Schuss
        private static int reloadTimerShot = 25;
        //Delay-Zeit beim Nachladen
        private static int reloadTimerMagazin = 200;
        //Timer der die Zeit runterechnent
        private static int reloadTimer = 0;
        //Magazin Größe
        private static int MagazinSize = 5;
        //Muinition im Magazin
        private static int Magazin = MagazinSize;       
        //Verhindert das schießen während dem Nachladen
        private static bool canShoot = true;
        //Ob ein Magazin benutzt werden soll (Wenn nein/false, ist reloadTimerMagazin = reloadTimerShot)
        private static bool useMagazin = true;


        public static void Update()
        {

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].position.Y <= -20)
                {
                    bullets.RemoveAt(i);
                }
            }


            if (Mouse.GetState().LeftButton == ButtonState.Pressed && canShoot)
            {

                //Spawned ein neues Netz
                bullets.Add(new Net(Player.CamPosition, Player.CamTarget));                
                //Schaut ob gerade die letzte Kugel verschossen wurde
                if (Magazin == 1 && useMagazin)
                {
                    Reload(reloadTimerMagazin, MagazinSize);
                }
                else Reload();
            }

            if (!canShoot) reloadTimer -= 1;
            if (reloadTimer < 0) canShoot = true;

            if (Keyboard.GetState().IsKeyDown(Keys.R)&&canShoot)
            {
                Reload(reloadTimerMagazin, MagazinSize);
            }



            foreach (Net net in bullets)
            {
                net.Update();
            }




        }
        public static void Draw()
        {
            foreach (Net net in bullets)
            {
                net.Draw();
            }
        }
        
        //Magazin Nachladen
        public static void Reload(int Time, int newMagazin)
        {
            canShoot = false;
            reloadTimer = Time;
            Magazin = newMagazin;
        }

        //"Nachladen" zwischen einzelnen Schüssen
        public static void Reload()
        {
            canShoot = false;
            reloadTimer = reloadTimerShot;
            Magazin -= 1;
        }

    }
}
