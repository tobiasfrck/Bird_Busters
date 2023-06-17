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
        public static List<Bird> birds;
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
        private static bool useMagazin = false;



        public static void Update()
        {

//          Shoot Mechanic
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && canShoot)
            {

                //Spawned ein neues Netz
                bullets.Add(new Net(FPSCamera.position, Player.CamTarget));                
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




//          Hit-Logic (change If Statement)
//          Marks Bullet und Bird, removes them later
//          Comment the Bullet Mark line to make the Net fly through Birds
            birds = BirdHandler.Birds;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = birds.Count - 1; j >= 0; j--)
                {
                    if (Vector3.Distance(bullets[i].position, birds[j]._position) <= 1)
                    {
//                        bullets[i].Marked = true;
                        birds[j].Marked = true;
                        Debug.WriteLine("hit");
                        Score.IncreaseScore();
                        Debug.WriteLine("New Score:" + Score.CurrentScore);
                    }
                }
            }

//          End of Logic
//
//          Update Bullets
            foreach (Net net in bullets)
            {
                net.Update();
            }

//          Removes Bullets When Marked or outside of the Game/Map
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].position.Y <= -20 || bullets[i].Marked)
                {
                    bullets.RemoveAt(i);
                }
            }

//          Removes Birds when marked
            for (int i = birds.Count - 1; i >= 0; i--)
            {
                if (birds[i].Marked)
                {
                    birds.RemoveAt(i);
                }
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
