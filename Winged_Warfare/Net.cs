using System;
using System.Data;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Winged_Warfare
{
    public class Net
    {
        public float gravity = 0.1f;
        public Vector3 position;
        public Vector3 target;


        public Net()
        {

        }
        public void spawn(Vector3 spawnPosition, Vector3 spawnTarget)
        {
            this.position = spawnPosition;
            this.target = spawnTarget;
        }

        public void update()
        {
            this.position = this.position + (this.target/10);
            Debug.WriteLine(this.position.X + "-"+this.position.Y + "-" + this.position.Z);
        }

        public void draw()
        {

        }

    }
}
