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
    internal class BulletHandler
    {
        //Liste mit allen Bullets
        public static List<Net> bullets = new List<Net>();
        public static List<Bird> birds;
        //Delay-Zeit zwischen Jedem Schuss
        private static int reloadTimerShot = 25;
        private static Timer _reloadTimerShot = new(1);
        //Delay-Zeit beim Nachladen
        private static int reloadTimerMagazin = 200;
        private static Timer _reloadTimerMagazin = new(1);
        //Timer der die Zeit runterechnent
        private static int reloadTimer = 0;
        //Magazin Größe
        private const int MagazinSize = 6;
        //Munition im Magazin
        private static int Magazin = MagazinSize;       
        //Verhindert das schießen während dem Nachladen
        private static bool canShoot = true;
        //Ob ein Magazin benutzt werden soll (Wenn nein/false, ist reloadTimerMagazin = reloadTimerShot)
        private static bool useMagazin = true;

        private static bool _isReloading = false;


        public static void Update()
        {
            
//          Shoot Mechanic
/*            if (Mouse.GetState().LeftButton == ButtonState.Pressed && canShoot && !(FPSCamera.IsMoving && FPSCamera.IsSprinting))
            {

                //Spawned ein neues Netz
                Game1.ShootEffect.Play(Game1.SFXVolume,0,0);
                bullets.Add(new Net(FPSCamera.position, Player.CamTarget));                
                //Schaut ob gerade die letzte Kugel verschossen wurde
                if (Magazin == 1 && useMagazin)
                {
                    Reload(reloadTimerMagazin, MagazinSize);
                }
                else Reload();
*///            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !_reloadTimerMagazin.IsRunning() && !_reloadTimerShot.IsRunning() && !(FPSCamera.IsMoving && FPSCamera.IsSprinting))
            {

                //Spawned ein neues Netz
                Game1.ShootEffect.Play(Game1.SFXVolume, 0, 0);
                bullets.Add(new Net(FPSCamera.position, Player.CamTarget));
                //Schaut ob gerade die letzte Kugel verschossen wurde
                canShoot = true;
                _isReloading = false;
                if (Magazin == 1 && useMagazin)
                {
                    _reloadTimerMagazin.SetTimeNRun(3000);
                    _isReloading=true;
                    canShoot = false;
                    Magazin = MagazinSize;
                }
                else
                {
                    _reloadTimerShot.SetTimeNRun(500);
                    Magazin -= 1;
                }
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && canShoot)
            {
                _reloadTimerMagazin.SetTimeNRun(3000);
                _isReloading=true;
                canShoot = false;
                Magazin = MagazinSize;
            }

            if (!_reloadTimerMagazin.IsRunning())
            {
                canShoot=true;
                _isReloading=false;
            }

            _reloadTimerShot.Update();
            _reloadTimerMagazin.Update();
//            if (!canShoot) reloadTimer -= 1;
/*            if (_reloadTimerMagazin.HasReached(0)&&_reloadTimerMagazin.IsRunning())
            {
                canShoot=true;
                _isReloading = false;
                Magazin = MagazinSize;
*///            }

            

            //TODO: frustrating, when mag shows its full but you cant shoot
            //Disables shooting when magazin is empty
//           if (Magazin == 0){
//                canShoot = false;
//            }
//

//          Hit-Logic (change If Statement)
//          Marks Bullet and Bird as not alive, removes them later
//          Comment the Bullet Mark line to make the Net fly through Birds
            birds = BirdSpawnpoint.Birds;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = birds.Count - 1; j >= 0; j--)
                {
                    if (Vector3.Distance(bullets[i].position, birds[j]._position) <= 1)
                    {
                        //bullets[i].Marked = true;
                        Game1.HitMarker.Play(Game1.SFXVolume/2, 0, 0);
                        birds[j].IsAlive = false;
                        bullets[i].HitCount++;
                        float playerDistanceToBird = birds[j].GetDistanceToPlayer();
                        float distanceMultplier = (float)Math.Pow(1.0155f, playerDistanceToBird);
                        Debug.WriteLine(birds[j].GetBirdStats());
                        Debug.WriteLine("hit from: "+ playerDistanceToBird);
                        Debug.WriteLine("volume: " + birds[j]._volumeMultiplier);
                        Score.IncreaseScore((int)(birds[j].GetBirdScore() * bullets[i].GetScoreMultiplier() * distanceMultplier));
                        Debug.WriteLine("New Score:" + Score.GetScore());
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
        }
        public static void Draw()
        {
            foreach (Net net in bullets)
            {
                net.Draw();
            }
        }
        
        //Magazin Nachladen
/*        public static void Reload(int Time, int newMagazin)
        {
            _isReloading = true;
            canShoot = false;
            reloadTimer = Time;
            Magazin = newMagazin;
*///        }

        //"Nachladen" zwischen einzelnen Schüssen
/*        public static void Reload()
        {
            canShoot = false;
            reloadTimer = reloadTimerShot;
            Magazin -= 1;
*///        }

        public static int GetAvailableShots()
        {
            return Magazin;
        }

        public static int GetMagazinSize()
        {
            return MagazinSize;
        }

        public static bool IsReloading()
        {
            return _isReloading;
        }

        public static void Reset()
        {
            Magazin = MagazinSize;
            canShoot = true;
            _isReloading = false;
            bullets.Clear();
        }
    }
}
