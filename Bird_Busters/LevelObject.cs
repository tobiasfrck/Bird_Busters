using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Bird_Busters
{
    internal class LevelObject
    {
        private Vector3 _rotation;
        private Vector3 _scale;
        private Vector3 _position;
        private Matrix _worldMatrix;
        private readonly int _line = 0; //line in the level file; used for debugging

        public LevelObject(Vector3 position, Vector3 rotation, Vector3 scale, int line)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            _line = line;
        }

        public LevelObject(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
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


        //Generates a new line for the level file, based on the current position, rotation and scale.
        public virtual string RegenerateLine()
        {
            return "No RegenerateLine() method found for object in line: " + Line;
        }

        public void Move(Vector3 moveVector)
        {
            Position += moveVector;
            UpdateWorldMatrix();
        }

        public void Rotate(Vector3 rotateVector)
        {
            Rotation += rotateVector;
            UpdateWorldMatrix();
        }

        public void ScaleObject(Vector3 scaleVector)
        {
            Scale += scaleVector;
            UpdateWorldMatrix();
        }

        private int Line
        {
            get => _line;
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateWorldMatrix();
            }
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                UpdateWorldMatrix();
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateWorldMatrix();
            }
        }

        public Matrix WorldMatrix
        {
            get => _worldMatrix;
        }
    }
}
