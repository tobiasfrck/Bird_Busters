using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int _birdID = 0;
        private static int _birdCount = 0;
        private DrawableObject _drawableObject;
        public bool IsAlive = true;
        private float _rndDirection = 0.1f; // in percent
        public Vector3 _position;

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
        private Vector3 _currentDirection;

        //No height needed, because height is variable
        private Vector2 _target;
        private float _targetTolerance;

        //No height needed, because height is variable
        private Vector2 _target2;

        public bool Marked;

        public Bird(Vector3 position, Vector3 rotation, Vector3 scale, Vector2 target, float targetTolerance,
            Vector2 target2)
        {
            _birdID = _birdCount;
            _birdCount++;

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
            _minSpeed = ((float)random.NextDouble() + 0.1f) / 5;
            _acceleration = _minSpeed;
            _speed = _minSpeed*2;
            _maxSpeed = ((float)random.NextDouble() + 0.5f) / 50;
            _minHeight = 2.5f;
            _maxHeight = 10f;
            _airResistanceSpeed = _minSpeed/10f;

            _target = target;
            _currentTarget = target;
            _targetTolerance = targetTolerance;
            _target2 = target2;


        }

        public void Update()
        {
            bool flap = false;
            _position = _drawableObject.Position;
            //if Bird is within the tolerance of the first target, it will fly to the second target
            if (Vector3.Distance(_drawableObject.Position, new Vector3(_target.X, _drawableObject.Position.Y, _target.Y)) < _targetTolerance && _currentTarget!=_target2)
            {
                //Debug.WriteLine("[Bird " + _birdId + "]: Reached Target1");
                _currentTarget = _target2;
                flap = true;
            }

            if (_speed <= _minSpeed)
            {
                flap=true;
                _speed += _acceleration;
                //Debug.WriteLine("[Bird "+_birdID"]: Flapped because of speed");
            }

            //if Bird is within the tolerance of the second target, it will die
            if (Vector3.Distance(_drawableObject.Position, new Vector3(_target2.X, _drawableObject.Position.Y, _target2.Y)) < _targetTolerance)
            {
                Debug.WriteLine("[Bird " + _birdID + "]: Reached Target2");
                IsAlive = false;
            }

            //if Bird is alive, it will flap its wings -> change in speed, direction and height
            if (IsAlive && flap)
            {
                FlapWings();
            }



            //apply air resistance
            _speed -= _airResistanceSpeed;

            //Apply gravity
            _drawableObject.Position = new Vector3(_drawableObject.Position.X, _drawableObject.Position.Y - _gravityAcceleration, _drawableObject.Position.Z);

            if (_speed < 0.0001)
            {
                Debug.WriteLine("Bird " + _birdID + " Speed: " + _speed);
                Debug.WriteLine("---");
            }

            //move to current direction
            _drawableObject.Move(_currentDirection * _speed);
        }

        public void FlapWings()
        {
            Random rnd = new Random();


            Vector3 currentSubTarget = new Vector3(_currentTarget.X, 0, _currentTarget.Y);

            //if Bird is too high or too low, it will fly to a random height between minHeight and maxHeight
            if (_drawableObject.Position.Y < _minHeight || _drawableObject.Position.Y > _maxHeight || rnd.Next(1)==0)
            {
                currentSubTarget.Y = _minHeight + (float)(rnd.NextDouble() * (_maxHeight - _minHeight));
            }
            

            //if Bird is outside of 2*targetTolerance range, it will fly with a random direction change to make it look more natural
            if (Vector3.Distance(_drawableObject.Position, currentSubTarget) > 2 * _targetTolerance)
            {
                Vector2 newSubTarget = new Vector2(currentSubTarget.X, currentSubTarget.Z);
                Vector2 position2D = new Vector2(_drawableObject.Position.X, _drawableObject.Position.Z);
                int k = 0;

                //While the new random direction is not somewhat towards the target do:
                do
                {
                    k++;
                    newSubTarget.X *= (float)(rnd.NextDouble() * rnd.Next(-1, 1) * (1f - _rndDirection));
                    newSubTarget.Y *= (float)(rnd.NextDouble() * rnd.Next(-1, 1) * (1f - _rndDirection));
                } while (Vector2.Distance(newSubTarget, _currentTarget) > Vector2.Distance(position2D, _currentTarget) && k < 10);

                //apply [_rndDirection]% random direction change if done in less than 10 tries
                if (k < 10)
                {
                    currentSubTarget.X *= newSubTarget.X;
                    currentSubTarget.Z *= newSubTarget.Y;
                }
            }
            Vector3 direction = currentSubTarget - _drawableObject.Position;

            direction.Normalize();
            _currentDirection = direction;

        }

        public void Draw()
        {
            _drawableObject.Draw();
        }
    }
}
