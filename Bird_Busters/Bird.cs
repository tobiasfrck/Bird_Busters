using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Bird_Busters
{
    public enum BirdType
    {
        Common = 0,
        Rare = 1,
        Legendary = 2
    }

    internal class Bird
    {
        private int _birdID;
        private static int _birdCount;
        private DrawableObject _drawableObject;
        public bool IsAlive = true;

        private readonly float _rndDirection = 15f; // in degree
        private Random _random = Game1.RandomGenerator;
        public Vector3 _position;

        private int _scorePoints = 10;
        private BirdType _birdType;

        private float _speed;
        private float _minSpeed;

        private float _acceleration;
        private float _airResistanceSpeed;
        private const float Gravity = -0.0005f;
        private float _yVelocity = -0.005f;

        private float _minHeight;
        private float _maxHeight;

        private float _distanceToPlayer;

        private Vector2 _offset;

        private Vector2 _currentTarget;
        private Vector3 _currentDirection;
        private float _targetTolerance;
        private PathPoint _pathPoint;

        //Audio
        private AudioEmitter Emitter;
        private SoundEffectInstance FlapEffectInstance;
        public float _volumeMultiplier = 1f;

        public Bird(PathPoint p, float targetTolerance)
        {
            Vector3 position = p.Position;

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
            _position = position;



            SetBirdType(DetermineBirdType(_random.NextDouble()));

            _targetTolerance = targetTolerance;
            GenerateCurrentTarget();

            Emitter = new AudioEmitter();
            Emitter.Up = new Vector3(0, 0, 1); //TODO: check if this is correct
            Emitter.Position = _drawableObject.Position;
            int soundIndex = _random.Next(Game1.BirdFlaps.Length);
            FlapEffectInstance = Game1.BirdFlaps[soundIndex].CreateInstance();
            FlapEffectInstance.Volume = Game1.SFXVolume / 100f;
            FlapEffectInstance.Apply3D(Game1.Listener, Emitter);
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

            //if Bird is below the minimum height, it will fly up
            if (_drawableObject.Position.Y < _minHeight)
            {
                //flap = true;
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
                if (FlapEffectInstance.State != SoundState.Playing)
                {
                    int soundIndex = _random.Next(Game1.BirdFlaps.Length);
                    FlapEffectInstance = Game1.BirdFlaps[soundIndex].CreateInstance();
                    FlapEffectInstance.Play();
                }
                FlapWings();
            }


            //apply air resistance
            _speed -= _airResistanceSpeed;

            //Apply gravity
            _drawableObject.Position = new Vector3(_drawableObject.Position.X, _drawableObject.Position.Y + _yVelocity, _drawableObject.Position.Z);
            _yVelocity += Gravity;

            if (_speed < 0.0001)
            {
                Debug.WriteLine("Bird " + _birdID + " Speed: " + _speed);
                Debug.WriteLine("---");
            }

            //move to current direction
            _drawableObject.Move(_currentDirection * _speed);
            _drawableObject.Rotation = GetLookAtRotation(_position, _drawableObject.Position);

            _distanceToPlayer = Vector3.Distance(_drawableObject.Position, Player.GetCamPosition());

            Emitter.Position = _drawableObject.Position;
            Emitter.Forward = _currentDirection; //TODO: check if this is correct
            _volumeMultiplier = 1f - (_distanceToPlayer / 40f); //TODO: Fine tune this
            _volumeMultiplier = MathHelper.Clamp(_volumeMultiplier, 0, 1);
            FlapEffectInstance.Volume = Game1.SFXVolume / 100f * _volumeMultiplier;
            FlapEffectInstance.Apply3D(Game1.Listener, Emitter);
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
                float heightGoal = _minHeight + (float)(_random.NextDouble() * (_maxHeight - _minHeight));
                float heightDifference = heightGoal - _drawableObject.Position.Y;
                if (_yVelocity <= 0)
                {
                    _yVelocity = heightDifference / (100f - _speed * 15f);
                    /*
                    if (_distanceToPlayer < 20f)
                    {
                        MenuManager.Instance.AddScoreIndicator(_drawableObject.Position, "Height added", _birdType);
                    }
                    */
                }
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
                    //Debug.WriteLine("[Bird " + _birdID + "]: Couldn't find a new direction.");
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

        public Vector3 GetLookAtRotationNoZ(Vector3 from, Vector3 to)
        {
            from.Z = 0;
            to.Z = 0;
            return GetLookAtRotation(from, to);
        }

        public String GetBirdStats()
        {
            return "Bird " + _birdID + " MinSpeed: " + _minSpeed + " minHeight: " + _minHeight + " Position: " + _drawableObject.Position;
        }

        public int GetBirdScore()
        {
            return _scorePoints;
        }
        public float GetDistanceToPlayer()
        {
            return _distanceToPlayer;
        }

        public Vector3 GetPosition()
        {
            return _drawableObject.Position;
        }

        public BirdType GetBirdType()
        {
            return _birdType;
        }

        private BirdType DetermineBirdType(double rand) => rand switch
        {
            < 0.04 => BirdType.Legendary,   // Legendary bird   Chance: 04%
            < 0.20 => BirdType.Rare,        // Rare bird        Chance: 16%
            _ => BirdType.Common            // Common bird      Chance: 80%
        };

        private void SetBirdType(BirdType type)
        {
            Vector3 rotation = new Vector3(0, 0, 0);
            Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
            switch (type)
            {
                case BirdType.Common:
                    _minSpeed = ((float)_random.NextDouble() + 0.3f) / 5f;
                    _acceleration = _minSpeed;
                    _speed = _minSpeed * 2;
                    _minHeight = 2.5f;
                    _maxHeight = 10f;
                    _airResistanceSpeed = _minSpeed / 10f;

                    _birdType = BirdType.Common;

                    _drawableObject = new DrawableObject(_position, rotation, scale, "Birds/Birb", -1);
                    break;
                case BirdType.Rare:
                    _minSpeed = ((float)_random.NextDouble() + 0.2f) / 4.5f;
                    _acceleration = _minSpeed;
                    _speed = _minSpeed * 2;
                    _minHeight = 3.5f;
                    _maxHeight = 10f;
                    _airResistanceSpeed = _minSpeed / 10f;

                    _scorePoints = 20;
                    _birdType = BirdType.Rare;

                    _drawableObject = new DrawableObject(_position, rotation, scale, "Birds/Birb2", -1);
                    break;
                case BirdType.Legendary:
                    _minSpeed = ((float)_random.NextDouble() + 0.5f) / 3f;
                    _acceleration = _minSpeed;
                    _speed = _minSpeed * 2;
                    _minHeight = 4.5f;
                    _maxHeight = 10f;
                    _airResistanceSpeed = _minSpeed / 10f;

                    _scorePoints = 60;
                    _birdType = BirdType.Legendary;

                    _drawableObject = new DrawableObject(_position, rotation, scale, "Birds/Birb3", -1);
                    break;
                default:
                    _drawableObject = new DrawableObject(_position, rotation, scale, "testContent/testCube", -1);
                    Debug.WriteLine("[Error]: BirdType not found");
                    break;
            }
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
