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
        public float gravity = 0.005f;
        public float velocity = 0f;
        public float speed = 1.5f;
        public Vector3 position;
        public Vector3 target;
        private DrawableObject _drawableObject;

        public Net(Vector3 spawnPosition, Vector3 spawnTarget)
        {
            _drawableObject = new DrawableObject(position, this.target, new Vector3(1, 1, 1), "testContent/Net", 50);
            this.position = spawnPosition;
            this.target = position - spawnTarget;
            this.target.Y += -0.09f;
            this.velocity = 0f;
        }


        public void Update()
        {
            this.position = this.position - ((this.target/2)*speed);
            this.position.Y = this.position.Y + this.velocity;
            velocity -= gravity;
            _drawableObject.Position = this.position;
            _drawableObject.Rotation = this.target;
            _drawableObject.Update();
      //      Debug.WriteLine(this.position.X + "-"+this.position.Y + "-" + this.position.Z);
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }

    }
}
