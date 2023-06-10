using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using static System.Formats.Asn1.AsnWriter;

namespace Winged_Warfare
{
    internal class Spawnpoint : LevelObject
    {
        private bool _isActive;

        private float _spawnRate; // birds per second
        private int _currSpawnedPerWave; // current birds per wave
        private int _maxSpawn; // max birds to spawn
        private Stopwatch _spawnTimer;

        private static int _currentlyAlive;
        private static int _spawnedTotal;
        private static int _maxAlive;

        private DrawableObject _debugDrawableObject;

        //TODO: Add more attributes to the spawnpoint

        public Spawnpoint(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position, rotation, scale, line)
        {
            _spawnTimer = new Stopwatch();
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

        public override void Draw()
        {
            if (Level.GetDebugMode())
            {
                _debugDrawableObject.Draw();
            }
        }

    }
}
