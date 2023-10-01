using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Bird_Busters
{
    internal class PathPoint : LevelObject
    {
        private int _pointID = 0;
        private BirdSpawnpoint _birdSpawnpoint;
        private List<PathPoint> _nextPoints;
        private Model _model;
        private List<int> _nextPointsId;
        private static Random rnd = Game1.RandomGenerator;

        public PathPoint(int pointID, Vector2 pos, int line) : base(new Vector3(pos.X, 4, pos.Y), Vector3.Zero, Vector3.One, line)
        {
            _pointID = pointID;
            _nextPoints = new List<PathPoint>();
            _nextPointsId = new List<int>();

            if (!Game1.Models.TryGetValue("testContent/testCube", out _model))
            {
                Debug.WriteLine("Model not found");
            }
        }

        public PathPoint(int pointID, Vector2 pos, BirdSpawnpoint sp, int line) : base(new Vector3(pos.X, 0, pos.Y), Vector3.Zero, Vector3.One, line)
        {
            _pointID = pointID;
            _birdSpawnpoint = sp;
            _nextPoints = new List<PathPoint>();
            _nextPointsId = new List<int>();
            if (!Game1.Models.TryGetValue("testContent/testCube", out _model))
            {
                Debug.WriteLine("Model not found");
            }
        }

        public override string RegenerateLine()
        {
            string line = "pathpoint," + _pointID + "," + Position.X + "," + Position.Z;

            foreach (PathPoint point in _nextPoints)
            {
                line += "," + point.GetPointID();
            }

            return line;
        }

        public override void Draw()
        {
            if (Level.GetDebugMode())
            {
                _model.Draw(Matrix.CreateTranslation(Position), Player.ViewMatrix, Player.ProjectionMatrix);
            }
        }

        public void SetSpawnpoint(BirdSpawnpoint sp)
        {
            _birdSpawnpoint = sp;
        }

        public int GetPointID()
        {
            return _pointID;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(Position.X, Position.Z);
        }

        public Vector3 GetPosition3D()
        {
            return Position;
        }


        public bool IsSpawnpoint()
        {
            return _birdSpawnpoint != null;
        }

        public void AddNextPoint(PathPoint point)
        {
            _nextPoints.Add(point);
        }

        public void AddNextPointID(int id)
        {
            _nextPointsId.Add(id);
        }

        public List<int> GetNextPointsID()
        {
            return _nextPointsId;
        }

        public PathPoint GetRandomNextPoint()
        {
            if (_nextPoints.Count == 0)
            {
                return null;
            }
            return _nextPoints[rnd.Next(_nextPoints.Count)];
        }

        public BirdSpawnpoint GetSpawnpoint()
        {
            return _birdSpawnpoint;
        }

        public bool IsLastPoint()
        {
            return _nextPoints.Count == 0;
        }

        public String GetNextPointsAsString()
        {
            String s = "";
            foreach (PathPoint point in _nextPoints)
            {
                s += point.GetPointID() + ", ";
            }
            return s;
        }

        public bool HasNextPoint()
        {
            return _nextPoints.Count > 0;
        }
    }
}
