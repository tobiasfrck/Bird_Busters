using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Winged_Warfare
{
    internal class Spawnpoint : LevelObject
    {
        private float _spawnRate;
        private int _maxSpawn;

        private static int _currentlyAlive;
        private static int _spawnedTotal;
        private static int _maxAlive;

        //TODO: Add more attributes to the spawnpoint

        public Spawnpoint(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position, rotation, scale, line)
        {
        }

        // Think of methods to spawn enemies
        // Where do we draw the enemies? Here?

    }
}
