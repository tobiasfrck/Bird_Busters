using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{

    public class Bird
    {
        private static int _birdId = 0;
        private DrawableObject _drawableObject;
        public bool _isAlive = true;
        private float _rndDirection = 0.0f; // in percent

        private float _speed;
        private float _minSpeed;
        private float _maxSpeed;

        private float _acceleration;
        private float _airResistanceSpeed;
        private float _gravityAcceleration;

        private float _minHeight;
        private float _maxHeight;
        private Random random = new Random();

        private Vector2 Offset = new Vector2();

        private Vector2 _currentTarget;

        //No height needed, because height is variable
        private Vector2 _target;
        private float _targetTolerance;

        //No height needed, because height is variable
        private Vector2 _target2;

        public Bird(Vector3 position, Vector3 rotation, Vector3 scale, Vector2 target, float targetTolerance,
            Vector2 target2)
        {

            // Vögel spawnen in einem Bereich anstatt auf der gleichen Stelle
            Offset.X = ((float)random.NextDouble() - 0.5f) * 2;
            Offset.Y = ((float)random.NextDouble() - 0.5f) * 2;

            position.X += Offset.X;
            position.Z += Offset.Y;
            
            // Vögel haben relativ zu ihrem Spawnpoint verschobene Targets (funktioniert irgenwie nicht)
            target.X += Offset.X;
            target.Y += Offset.Y;

            target2.X += Offset.X;
            target2.Y += Offset.Y;

            _drawableObject = new DrawableObject(position, rotation, scale, "testContent/testCube", -1);

            //Bird bekommt random Stats
            _acceleration = (float)random.NextDouble() + 0.01f;
            _speed = ((float)random.NextDouble()+0.2f)/50;
            _minSpeed = ((float)random.NextDouble() + 0.1f) / 50;
            _maxSpeed = ((float)random.NextDouble() + 0.5f) / 50;
            _minHeight = position.Y+ (float)random.NextDouble()*-2f;
            _maxHeight = position.Y+ (float)random.NextDouble()*2f;
            _airResistanceSpeed = (float)random.NextDouble();

            _target = target;
            _currentTarget = target;
            _targetTolerance = targetTolerance;
            _target2 = target2;


        }

        public void Update()
        {
            //if Bird is within the tolerance of the first target, it will fly to the second target
            if (Vector3.Distance(_drawableObject.Position, new Vector3(_target, _drawableObject.Position.Y)) < _targetTolerance)
            {
                _currentTarget = _target2;
            }


            if (_speed < _minSpeed)
            {
                _speed += _acceleration;
            }

            //if Bird is within the tolerance of the second target, it will die
            if (Vector3.Distance(_drawableObject.Position, new Vector3(_target, _drawableObject.Position.Y)) < _targetTolerance)
            {
                _isAlive = false;
            }

            if (_isAlive)
            {
                FlapWings();
            }

            //apply air resistance
            _speed -= _airResistanceSpeed;

            //Apply gravity
            _drawableObject.Position = new Vector3(_drawableObject.Position.X, _drawableObject.Position.Y - _gravityAcceleration, _drawableObject.Position.Z);
        }

        //move the bird towards the current target
        private void FlapWings()
        {
            Random rnd = new Random();

            Vector3 currentSubTarget = new Vector3(_currentTarget, 0);

            //if Bird is too high or too low, it will fly to a random height between minHeight and maxHeight
            if (_drawableObject.Position.Y < _minHeight || _drawableObject.Position.Y > _maxHeight)
            {
                currentSubTarget.Y = _minHeight + rnd.Next(0, (int)(_maxHeight - _minHeight));
            }

            Vector3 direction = currentSubTarget - _drawableObject.Position;

            direction.Normalize();

            if (Vector3.Distance(_drawableObject.Position, currentSubTarget) > 2 * _targetTolerance)
            {
                //apply [_rndDirection]% random direction change
                direction.X *= (float)(rnd.NextDouble() * rnd.Next(-1, 1) * (1f - _rndDirection));
                direction.Y *= (float)(rnd.NextDouble() * rnd.Next(-1, 1) * (1f - _rndDirection));
            }

            _drawableObject.Position += direction * _speed;
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }
    }
}
