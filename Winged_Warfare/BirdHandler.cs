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



    public class BirdHandler
    {
        public static List<Bird> Birds = new List<Bird>();
        private static int MaxBirdCount = 20000;
        private static int CurBirdCount = 0;
        private static int SpawnCooldown = 1;
        private static int Timer = 0;

        public static void Update()
        {
            if (Timer > 0) Timer--;
            if (CurBirdCount < MaxBirdCount && Timer <= 0) {
                Birds.Add(new Bird(new Vector3(0, 1, 0), new Vector3(1, 1, 1), new Vector3(0.2f, 0.2f, 0.2f), new System.Numerics.Vector2(50, 10),5, new System.Numerics.Vector2(70, -50)));
                CurBirdCount++;
                Timer = SpawnCooldown;
            }


            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                if (!Birds[i].IsAlive)
                {
                    Birds.RemoveAt(i);
                }
            }



            foreach (Bird bird in Birds)
            {
                bird.Update();
            }
        }
        public static void Draw()
        {
            foreach (Bird bird in Birds)
            {
                bird.Draw();
            }
        }


    }
}
