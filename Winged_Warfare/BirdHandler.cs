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
        private static int MaxBirdCount = 40;
        private static int CurBirdCount = 0;
        private static int SpawnCooldown = 50;
        private static int Timer = 0;
        private static Random Random = new Random();

        // Target 1 (1. Checkpoint der Vögel)
        private static System.Numerics.Vector2 Target1 = new System.Numerics.Vector2(7,0);

        // Target 2 (Ziele der Vögel)
        private static System.Numerics.Vector2 Target2p1 = new System.Numerics.Vector2(7, 500);
        private static System.Numerics.Vector2 Target2p2 = new System.Numerics.Vector2(700, 0);
        private static System.Numerics.Vector2 Target2p3 = new System.Numerics.Vector2(7, -500);

        private static List<System.Numerics.Vector2> TargetList = new List<System.Numerics.Vector2>();


        //Change this to get the Target Points from a File
        public static void CreateList()
        {
            TargetList.Add(Target2p1);
            TargetList.Add(Target2p2);
            TargetList.Add(Target2p3);
        }

        public static void Update()
        {


            if (Timer > 0) Timer--;
            if (CurBirdCount < MaxBirdCount && Timer <= 0) {

                //Vögel bekommen ein zufälliges Ziel zugelost
                int TargetNumber = Random.Next(TargetList.Count);

                Birds.Add(new Bird(new Vector3(-50, 3, 0), new Vector3(1, 1, 1), new Vector3(0.2f, 0.2f, 0.2f), Target1 ,5, TargetList[TargetNumber]));
                CurBirdCount++;
                Timer = SpawnCooldown;
            }

            foreach (Bird bird in Birds)
            {
                bird.Update();
            }

            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                if (!Birds[i].IsAlive)
                {
                    Birds.RemoveAt(i);
                }
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
