using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;

namespace Winged_Warfare
{
    internal class Spawnpoint : LevelObject
    {
        private int _spawnpointID = 0;
        private bool _isActive;

        private float _spawnRate; // birds per second
        private int _currSpawnedPerWave; // current birds per wave
        private int _maxSpawn; // max birds to spawn
        private Stopwatch _spawnTimer;

        private static int _currentlyAlive;
        private static int _spawnedTotal;
        private static int _maxAlive;

        private PathPoint _pathPoint;
        private Model _model;

        //TODO: Add more attributes to the spawnpoint

        public Spawnpoint(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position, rotation, scale, line)
        {
            _spawnTimer = new Stopwatch();
            if (!Game1.Models.TryGetValue("testContent/testCube", out _model))
            {
                Debug.WriteLine("Model not found");
            }
        }

        // TODO: Think of methods to spawn enemies
        // Where do we draw the birds? Here?

        public override void Update()
        {
            SpawnBird();
        }

        private void SpawnBird()
        {
            if (!_isActive)
            {
                _spawnTimer.Start();
                _isActive = true;
                return;
            }

            if (_spawnTimer.ElapsedMilliseconds >= 1000 / _spawnRate)
            {
                _spawnTimer.Restart();
                _currSpawnedPerWave = 0;
            }




            if (_currentlyAlive < _maxAlive)
            {

            }

        }

        private void startSpawnTimer()
        {
            
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
        }

        public int GetSpawnpointID()
        {
            return _spawnpointID;
        }



        public void SetPathPoint(PathPoint pathPoint)
        {
            _pathPoint = pathPoint;
        }

        private PathPoint GetPathPoint()
        {
            return _pathPoint;
        }

        public override void Draw()
        {
            if (Level.GetDebugMode())
            {
                _model.Draw(Matrix.CreateTranslation(Position), Player.ViewMatrix, Player.ProjectionMatrix);
            }
        }

    }
}
