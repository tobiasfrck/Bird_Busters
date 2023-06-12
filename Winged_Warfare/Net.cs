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
        public Vector3 lastPosition;
        public Vector3 target;
        private DrawableObject _drawableObject;
        public bool Marked = false;

        public Net(Vector3 spawnPosition, Vector3 spawnTarget)
        {
            _drawableObject = new DrawableObject(position, this.target, new Vector3(1, 1, 1), "testContent/Net", 50);
            this.position = spawnPosition;
            this.lastPosition = spawnPosition;
            this.target = position - spawnTarget;
            this.target.Y += -0.09f;
            this.velocity = 0f;
        }


        public void Update()
        {
            this.lastPosition = this.position;
            this.position = this.position - ((this.target/2)*speed);
            this.position.Y = this.position.Y + this.velocity;
            velocity -= gravity;

            Vector3 direction = this.position - this.lastPosition;
            Vector3.Normalize(direction);       

            _drawableObject.Position = this.position;
            _drawableObject.Rotation = Rad2Deg(direction);
            _drawableObject.Update();
      //      Debug.WriteLine(this.position.X + "-"+this.position.Y + "-" + this.position.Z);
        }

        private static Vector3 Rad2Deg(Vector3 rad)
        {
            Vector3 deg = new Vector3();
            deg.X = MathHelper.ToDegrees(rad.X);
            deg.Y = MathHelper.ToDegrees(rad.Y);
            deg.Z = MathHelper.ToDegrees(rad.Z);
            return deg;
        }

        public static Vector3 DirToRotation(Vector3 dir)
        {
            Vector3 rotation = new Vector3();
            float angleX = MathF.Acos(Vector3.Dot(dir, Vector3.Up));
            float angleY = MathF.Acos(Vector3.Dot(dir, Vector3.Up));
            float angleZ = MathF.Acos(Vector3.Dot(dir, Vector3.Up));

            rotation.X = MathHelper.ToDegrees(angleX);
            rotation.Y = MathHelper.ToDegrees(angleY);
            rotation.Z = MathHelper.ToDegrees(angleZ);
            return rotation;
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }

    }
}
