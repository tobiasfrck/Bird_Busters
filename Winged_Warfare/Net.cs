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
        private DrawableObject _drawableObject;

        public Net()
        {
            _drawableObject = new DrawableObject(position, new Vector3(0, 0, 0), new Vector3(1, 1, 1), "testContent/testCube", 50);

        }
        public void Spawn(Vector3 spawnPosition, Vector3 spawnTarget)
        {
            _drawableObject = new DrawableObject(position, new Vector3(0, 0, 0), new Vector3(1, 1, 1), "testContent/testCube", 50);
            this.position = spawnPosition;
            this.target = spawnTarget;
        }

        public void Update()
        {
            this.position = this.position + (this.target/10);
            _drawableObject.Position = this.position;
            _drawableObject.UpdateWorldMatrix();
            _drawableObject.Update();
            Debug.WriteLine(this.position.X + "-"+this.position.Y + "-" + this.position.Z);
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }

    }
}
