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
    public class VectorCube
    {
        private Vector3 _min;
        private Vector3 _max;

        public VectorCube(Vector3 min, Vector3 max)
        {
            float minX = Math.Min(min.X, max.X);
            float minY = Math.Min(min.Y, max.Y);
            float minZ = Math.Min(min.Z, max.Z);
            _min = new Vector3(minX, minY, minZ);

            float maxX = Math.Max(min.X, max.X);
            float maxY = Math.Max(min.Y, max.Y);
            float maxZ = Math.Max(min.Z, max.Z);
            _max = new Vector3(maxX, maxY, maxZ);
        }

        public bool IsIn(Vector3 point)
        {
            return (point.X >= _min.X && point.X <= _max.X) && (point.Y >= _min.Y && point.Y <= _max.Y) && (point.Z >= _min.Z && point.Z <= _max.Z);
        }

        public Vector3 GetStart()
        {
            return _min;
        }

        public Vector3 GetEnd()
        {
            return _max;
        }
    }
    

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
        
        public static MenuManager _menuManager;

        private static List<VectorCube> _deletionZones = new List<VectorCube>();

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
                    _isReloading = true;
                    canShoot = false;
                    Magazin = MagazinSize;
                    //Spielt Nachlade-Soundeffekt ab
                    Game1.Reload_Mag.Play();
                }
                else
                {
                    _reloadTimerShot.SetTimeNRun(500);
                    Magazin -= 1;
                }
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && Magazin!=MagazinSize && canShoot)
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
                        int score = (int)(birds[j].GetBirdScore() * bullets[i].GetScoreMultiplier() * distanceMultplier);
                        Score.IncreaseScore(score, birds[j].GetBirdType());
                        _menuManager.AddScoreIndicator(birds[j]._position, score.ToString(), birds[j].GetBirdType());
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

            //TODO: optimize
//          Removes Bullets When Marked or outside of the Game/Map
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                if (bullets[i].position.Y <= -20 || bullets[i].Marked)
                {
                    bullets.RemoveAt(i);
                    continue;
                }

                foreach (var deletionZone in _deletionZones)
                {
                    if (deletionZone.IsIn(bullets[i].position))
                    {
                        bullets.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        public static void Draw()
        {
            foreach (Net net in bullets)
            {
                net.Draw();
            }

            for (int i = _deletionZones.Count - 1; Level.GetDebugMode() && i >= 0; i--)
            {
                DrawableObject _zonePartA = new DrawableObject(_deletionZones[i].GetStart(), new (), new(1,1,1),"testContent/testCube");
                DrawableObject _zonePartB = new DrawableObject(_deletionZones[i].GetEnd(), new(), new(1, 1, 1), "testContent/testCube");
                _zonePartA.Draw();
                _zonePartB.Draw();
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

        public static void CreateDeletionZoneList()
        {
            //Next to the Player
            _deletionZones.Add(new(new(-2.948f, 0, -11.381183f),new(-15.441346f,16.999987f,24.57319f)));
            //Back of the level
            _deletionZones.Add(new(new(44.85267f, 22.759964f, -43.36434f),new(-27.077879f,-1.4400704f,-25.545025f)));
            //Cyan house
            _deletionZones.Add(new(new(43.46f, -2.5600688f, 1.5724019f),new(12.39417f, 15.039919f, -10.7506f)));
            //Front of the level
            _deletionZones.Add(new(new(41.445988f, 15.5601f, 69.35201f),new(-39.8f, -2, 83.00f)));
            //To the left of the houses next to the player
            _deletionZones.Add(new(new(-54.28587f, 0f, 53.17f),new(-3.7761f, 15.92f, 39.001f)));
            //other cyan house on the corner
            _deletionZones.Add(new(new(12.91f, 11.08f, 37.988857f),new(71.733894f, -1f, 58.88f)));
            //small house next to the player
            _deletionZones.Add(new(new(13.360575f,3.04f,25.149f),new(25.421f,-1f,17.171f)));

            //green house with the corner
            _deletionZones.Add(new(new(34.191f, -1f, -0.757f), new(42.819f, 6.56f, 19.243f)));
            //big house behind the green house
            _deletionZones.Add(new(new(43.49f,-1f,24.2f), new(66.58f,26.36f,0.5248f)));
        }
    }
}
