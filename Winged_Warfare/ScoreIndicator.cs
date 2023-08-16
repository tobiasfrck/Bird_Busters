﻿using System.Diagnostics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Winged_Warfare
{
    internal class ScoreIndicator
    {
        private readonly Timer _timer;
        private readonly int _score;
        private readonly Color _scoreColor = Color.White;
        private readonly Vector3 _origin;
        private float _yOffset;

        public ScoreIndicator(Vector3 origin, int score, BirdType birdType)
        {
            _timer = new Timer(1000);
            _score = score;
            _origin = origin;
            _yOffset = 0;

            switch (birdType)
            {
                case BirdType.Common:
                    _scoreColor = Color.White;
                    break;
                case BirdType.Rare:
                    _scoreColor = Color.Red;
                    break;
                case BirdType.Legendary:
                    _scoreColor = Color.Orange;
                    break;
            }
        }

        public void Update()
        {
            _timer.Update();
            _yOffset -= 0.05f;
        }

        private static Vector2 WorldToScreen(Vector3 worldPosition)
        {
            // Transform the world position by the view and projection matrices
            Vector3 transformedPosition = Vector3.Transform(worldPosition, Player.ViewMatrix * Player.ProjectionMatrix);

            Debug.WriteLine("Z: " + transformedPosition.Z);
            if (transformedPosition.Z < 0)
            {
                return new Vector2(-1, -1);
            }

            // Normalize the transformed position
            Vector2 normalizedPosition = new Vector2(transformedPosition.X / transformedPosition.Z, transformedPosition.Y / transformedPosition.Z);

            // Convert the normalized position to screenspace
            Vector2 screenspacePosition = new Vector2(
                (normalizedPosition.X + 1) * 0.5f * Game1.Width,
                (1 - normalizedPosition.Y) * 0.5f * Game1.Height);

            return screenspacePosition;
        }

        public Vector2 GetScreenPosition()
        {
            return WorldToScreen(_origin) + new Vector2(0, _yOffset);
        }

        public int GetScore()
        {
            return _score;
        }

        public Color GetScoreColor()
        {
            return new Color(_scoreColor* (1f - _timer.GetProgress()),1f-_timer.GetProgress());
        }

        public bool AnimationDone()
        {
            return !_timer.IsRunning();
        }
    }
}
