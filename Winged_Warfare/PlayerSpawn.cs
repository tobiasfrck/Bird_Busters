using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Winged_Warfare
{

    //Maybe not needed but I'm keeping it for now because it's different from the other spawnpoints
    internal class PlayerSpawn : LevelObject
    {
        public PlayerSpawn(Vector3 position, Vector3 rotation, Vector3 scale, int line) : base(position, rotation,
            scale, line)
        {
            UpdateWorldMatrix();
        }

        public override void Draw()
        {
            //Debug.WriteLine("This is the player spawn. No drawing needed.");
        }

        public override void Update()
        {

        }
    }
}
