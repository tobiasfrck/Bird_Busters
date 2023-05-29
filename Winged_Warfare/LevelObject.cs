using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{
    internal class LevelObject
    {
        private Vector3 _rotation;
        private Vector3 _scale;
        private Vector3 _position;
        private Matrix _worldMatrix;
        private int _line = 0; //line in the level file; used for debugging

        public LevelObject(Vector3 position, Vector3 rotation, Vector3 scale, int line)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            _line = line;
        }

        /// <summary>
        /// Call this method after changing the position, rotation or scale of the object, to update the world matrix.
        /// </summary>
        public void UpdateWorldMatrix()
        {
            _worldMatrix = Matrix.CreateScale(Scale) * Matrix.CreateRotationX(MathHelper.ToRadians(Rotation.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) * Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation.Z)) * Matrix.CreateTranslation(Position);
        }

        public virtual void Draw()
        {
            //Debug.WriteLine("No Draw() method found for object in line: " + Line);
        }

        public virtual void Update()
        {
            //Debug.WriteLine("No Update() method found for object in line: " + Line);
        }

        public int Line
        {
            get => _line;
            set => _line = value;
        }

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public Vector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public Matrix WorldMatrix
        {
            get => _worldMatrix;
            set => _worldMatrix = value;
        }
    }
}
