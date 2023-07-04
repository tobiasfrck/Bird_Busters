using System;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector3 = Microsoft.Xna.Framework.Vector3;

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
            _drawableObject = new DrawableObject(position, this.target, new Vector3(1, 1, 1), "8Ball_Net", 50);
            this.position = spawnPosition;
            this.position.Y -= 0.75f;
            this.lastPosition = spawnPosition;
            this.target = -spawnTarget;
            this.target.Y += -0.09f;
            this.velocity = 0f;
            RotateNet();
        }


        public void Update()
        {
            this.lastPosition = this.position;
            this.position = this.position - ((this.target/2)*speed);
            this.position.Y = this.position.Y + this.velocity;
            velocity -= gravity;


            _drawableObject.Position = this.position;
            RotateNet();
            _drawableObject.Update();
      //      Debug.WriteLine(this.position.X + "-"+this.position.Y + "-" + this.position.Z);
        }

        private void RotateNet()
        {
            Matrix lookAtMatrix = Matrix.CreateLookAt(this.lastPosition, this.position, Vector3.Up);

            Vector3 rotation = new()
            {
                X = (float)Math.Atan2(lookAtMatrix.M32, lookAtMatrix.M33),
                Y = (float)Math.Atan2(-lookAtMatrix.M31,
                    Math.Sqrt(lookAtMatrix.M32 * lookAtMatrix.M32 + lookAtMatrix.M33 * lookAtMatrix.M33)),
                Z = (float)Math.Atan2(lookAtMatrix.M21, lookAtMatrix.M11)
            };

            _drawableObject.Rotation = Rad2Deg(rotation);
        }


        private static Vector3 Rad2Deg(Vector3 rad)
        {
            Vector3 deg = new()
            {
                X = MathHelper.ToDegrees(rad.X),
                Y = MathHelper.ToDegrees(rad.Y),
                Z = MathHelper.ToDegrees(rad.Z)
            };
            return deg;
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }

    }
}
