using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{

    internal class Bird
    {
        private int _birdID;
        private static int _birdCount;
        private DrawableObject _drawableObject;
        public bool IsAlive = true;
        private readonly float _rndDirection = 15f; // in degree
        public Vector3 _position;

        private float _speed;
        private float _minSpeed;
        private float _maxSpeed;

        private float _acceleration;
        private float _airResistanceSpeed;
        private float _gravityAcceleration = 0.005f;

        private float _minHeight;
        private float _maxHeight;
        private Random _random = new();

        private Vector2 _offset;

        private Vector2 _currentTarget;
        private Vector3 _currentDirection;

        private float _targetTolerance;

        private PathPoint _pathPoint;

        public Bird(PathPoint p, float targetTolerance)
        {
            Vector3 position = p.Position;
            Vector3 rotation = new Vector3(0, 0, 0);
            Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);

            _birdID = _birdCount;
            _birdCount++;
            _pathPoint = p;
            if (_birdID == 0)
            {
                PrintPath();
            }

            // Vögel spawnen in einem Bereich anstatt auf der gleichen Stelle
            _offset.X = ((float)_random.NextDouble() - 0.5f) * 2;
            _offset.Y = ((float)_random.NextDouble() - 0.5f) * 2;

            position.X += _offset.X;
            position.Z += _offset.Y;

            _drawableObject = new DrawableObject(position, rotation, scale, "testContent/testCube", -1);

            //Bird bekommt random Stats
            _minSpeed = ((float)_random.NextDouble() + 0.2f) / 5;
            _acceleration = _minSpeed;
            _speed = _minSpeed * 2;
            _maxSpeed = ((float)_random.NextDouble() + 0.5f) / 50;
            _minHeight = 2.5f;
            _maxHeight = 10f;
            _airResistanceSpeed = _minSpeed / 10f;

            _targetTolerance = targetTolerance;
            GenerateCurrentTarget();
        }

        public void Update()
        {
            bool flap = false;
            _position = _drawableObject.Position;
            //if Bird is within the tolerance of the first target, it will fly to the second target
            if (Vector3.Distance(_drawableObject.Position, new Vector3(_currentTarget.X, _drawableObject.Position.Y, _currentTarget.Y)) < _targetTolerance * 1 && _pathPoint.HasNextPoint())
            {
                //Debug.WriteLine("[Bird " + _birdId + "]: Reached Target1");
                _pathPoint = _pathPoint.GetRandomNextPoint();
                GenerateCurrentTarget();
            }

            if (_speed <= _minSpeed)
            {
                flap = true;
                _speed += _acceleration;
                //Debug.WriteLine("[Bird "+_birdID"]: Flapped because of speed");
            }

            //if Bird is within the tolerance of the last target, it will die
            if (_pathPoint.IsLastPoint() && Vector3.Distance(_drawableObject.Position, new Vector3(_pathPoint.GetPosition().X, _drawableObject.Position.Y, _pathPoint.GetPosition().Y)) < _targetTolerance)
            {
                //Debug.WriteLine("[Bird " + _birdID + "]: Reached the last target.");
                IsAlive = false;
            }

            //if Bird is alive, it will flap its wings -> change in speed, direction and height
            if (IsAlive && flap)
            {
                int index = _random.Next(0,150);
                if (index < 1) {
                    float distance = Vector3.Distance(this._position,Player.GetCamPosition());
                    //Debug.WriteLine(distance);
                    distance = distance / 100;
                    float FixedVolume = (Game1.Volume - distance);
                    if(FixedVolume>0)
                        Game1.BirdFlaps.Play(FixedVolume, 0, 0);
                    //Debug.WriteLine(distance + " / "+FixedVolume);
                }
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

            _drawableObject.Rotation = GetLookAtRotation(_position, _drawableObject.Position);
        }

        private void PrintPath()
        {
            PathPoint p = _pathPoint;
            while (p != null)
            {
                Debug.WriteLine(p.GetPosition());
                p = p.GetRandomNextPoint();
            }
        }

        private void FlapWings()
        {
            Vector2 position2D = new Vector2(_drawableObject.Position.X, _drawableObject.Position.Z);
            Vector3 currentSubTarget = new Vector3(_currentTarget.X, _drawableObject.Position.Y, _currentTarget.Y);

            //if Bird is too low, it will flap to a random height between minHeight and maxHeight
            if (_drawableObject.Position.Y < _minHeight || (_random.NextDouble() >= 0.75 && (_minHeight + _maxHeight) / 2 > _drawableObject.Position.Y))
            {
                currentSubTarget.Y = _minHeight + (float)(_random.NextDouble() * (_maxHeight - _minHeight));
            }


            //if Bird is outside of 2*targetTolerance range, it will fly with a random direction change to make it look more natural
            if (Vector3.Distance(_drawableObject.Position, currentSubTarget) > 1 * _targetTolerance)
            {
                Vector2 newSubTarget = new();
                int k = 0;

                //While the new random direction is not somewhat towards the target do:
                do
                {
                    k++;
                    float angle = (float)(_random.NextDouble() * _random.Next(-1, 1)) * _rndDirection;
                    newSubTarget.X = (float)(Math.Cos(angle * currentSubTarget.X) - Math.Sin(angle * currentSubTarget.Z));
                    newSubTarget.Y = (float)(Math.Sin(angle * currentSubTarget.X) + Math.Cos(angle * currentSubTarget.Z));
                } while (Vector2.Distance(newSubTarget, _currentTarget) * 1.1f > Vector2.Distance(position2D, _currentTarget) && k < 10);

                //apply [_rndDirection]% random direction change if done in less than 10 tries
                if (k < 10)
                {
                    currentSubTarget.X += newSubTarget.X;
                    currentSubTarget.Z += newSubTarget.Y;
                }
                else
                {
                    //Debug.WriteLine("Bird " + _birdID + " couldn't find a new direction");
                    //NewSubTarget
                    //Debug.WriteLine("CurrentSubTarget: " + currentSubTarget);
                    //Debug.WriteLine("CurrentTarget: " + _currentTarget);
                    //Debug.WriteLine("NewSubTarget: " + newSubTarget);
                }
            }
            Vector3 direction = currentSubTarget - _drawableObject.Position;

            direction.Normalize();
            _currentDirection = direction;

        }

        //Generates a random target within the targetTolerance radius around the current target
        private void GenerateCurrentTarget()
        {
            _currentTarget = _pathPoint.GetPosition();
            float r = _targetTolerance * (float)Math.Sqrt(_random.NextDouble());
            float theta = (float)(_random.NextDouble() * 2 * MathHelper.Pi);
            _currentTarget.X += r * (float)Math.Cos(theta);
            _currentTarget.Y += r * (float)Math.Sin(theta);
        }

        public void Draw()
        {
            _drawableObject.Draw();
        }

        //returns the rotation vector to look from "from" to "to" in degrees
        private Vector3 GetLookAtRotation(Vector3 from, Vector3 to)
        {
            Matrix lookAtMatrix = Matrix.CreateLookAt(from, to, Vector3.Up);

            Vector3 rotation = new()
            {
                X = (float)Math.Atan2(lookAtMatrix.M32, lookAtMatrix.M33),
                Y = (float)Math.Atan2(-lookAtMatrix.M31,
                    Math.Sqrt(lookAtMatrix.M32 * lookAtMatrix.M32 + lookAtMatrix.M33 * lookAtMatrix.M33)),
                Z = (float)Math.Atan2(lookAtMatrix.M21, lookAtMatrix.M11)
            };

            return Rad2Deg(rotation);
        }

        public String GetBirdStats()
        {
            return "Bird " + _birdID + " MinSpeed: " + _minSpeed + " minHeight: " + _minHeight + " Position: " + _drawableObject.Position;
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
    }
}
