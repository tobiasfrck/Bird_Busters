using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace Bird_Busters
{
    internal class BirdSpawnpoint : LevelObject
    {
        public static List<Bird> Birds = new List<Bird>();

        private static bool _firstSpawnpointSpawned = false;
        private bool _isFirstSpawnpoint = false;
        private int _spawnpointID = 0;
        private bool _isActive;

        private float _frames = 0f;
        private int _secondsUntilApplyCurrSRMultilplier = 0; //standard 30 seconds

        private float _spawnRate; // birds per second
        private float _currSpawnRateMultiplier = 0.3f; // used to store multiplier that increases spawn rate over time
        private float _tempSpawnRateMultiplier = 0.2f; // used to store multipliers of special events
        private float _spawnTank = 2f; // the "tank" is filled each frame and when it reaches >1, n>=1 birds are spawned

        private Random _random;

        private PathPoint _pathPoint;
        private Model _model;

        //TODO: Add more attributes to the spawnpoint

        //Spawnpoints are positioned on initialization in origin.
        //They are then moved to the position of the pathpoint they are assigned to.
        public BirdSpawnpoint(int spawnpointId, float spawnRate, float SpawnRateMultiplier, int line) : base(new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(1,1,1), line)
        {
            if (_firstSpawnpointSpawned == false)
            {
                _isFirstSpawnpoint = true;
                _firstSpawnpointSpawned = true;
            }
            _spawnpointID = spawnpointId;
            _spawnRate = spawnRate;
            _currSpawnRateMultiplier = SpawnRateMultiplier;

            if (!Game1.Models.TryGetValue("testContent/testCube", out _model))
            {
                Debug.WriteLine("[Spawnpoint Bird]: Model not found");
            }

            _random = Game1.RandomGenerator;
        }

        public override void Update()
        {
            float spawnAmount = (_spawnRate / Game1.Fps) * _tempSpawnRateMultiplier;
            spawnAmount *= (float)(_random.NextDouble() * 1.5f + 0.5f);
            _spawnTank += spawnAmount;
            SpawnBird();

            //first spawnpoint is responsible for updating birds
            if (_isFirstSpawnpoint)
            {
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

        }

        private void SpawnBird()
        {
            if (_spawnTank >= 1)
            {
                int birdsToSpawn = (int) Math.Floor(_spawnTank);
                _spawnTank -= birdsToSpawn;
                for (int i = 0; i < birdsToSpawn; i++)
                {
                    Birds.Add(new Bird(_pathPoint, 3f));
                }
            }
        }

        public override String RegenerateLine()
        {
            return "spawnpoint," + _spawnpointID + "," + _spawnRate + "," + _currSpawnRateMultiplier;
        }

        public int GetSpawnpointID()
        {
            return _spawnpointID;
        }



        public void SetPathPoint(PathPoint pathPoint)
        {
            _pathPoint = pathPoint;
            Position = _pathPoint.GetPosition3D();
        }

        private PathPoint GetPathPoint()
        {
            return _pathPoint;
        }

        // Use Reset only when you reload the level
        public static void Reset()
        {
            Birds.Clear();
            _firstSpawnpointSpawned = false;
        }

        public override void Draw()
        {
            //TODO: optimize this
            _frames++;

            if (_frames >= Game1.Fps)
            {
                //_secondsUntilApplyCurrSRMultilplier++;
                _frames = 0;
            }

            if (Level.GetDebugMode())
            {
                _model.Draw(Matrix.CreateTranslation(Position), Player.ViewMatrix, Player.ProjectionMatrix);
            }

            if (_isFirstSpawnpoint)
            {
                foreach (Bird bird in Birds)
                {
                    bird.Draw();
                }
            }
        }
    }
}
