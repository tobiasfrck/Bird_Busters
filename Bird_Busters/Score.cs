using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bird_Busters
{
    public class Score
    {
        private static int _currentScore = 0;
        private static int _highscore;
        private static bool _isNewHighscore = false;
        private static int[] _birdsHit = new int[Enum.GetNames(typeof(BirdType)).Length]; // Breakdown of how many of each bird type has been hit

        public static void IncreaseScore(int i, BirdType type)
        {
            _currentScore += i;
            _birdsHit[(int)type]++;
            if (_currentScore > _highscore && !BulletHandler.isCheatActivated())
            {
                SetHighscore(_currentScore);
                _isNewHighscore = true;
            }
        }

        public static void ResetScore()
        {
            _currentScore = 0;
            _isNewHighscore = false;
            _birdsHit = new int[Enum.GetNames(typeof(BirdType)).Length];
        }

        public static int GetScore()
        {
            return _currentScore;
        }

        public static void SetHighscore(int score)
        {
            _highscore = score;
            Level.SaveHighscore(score);
        }

        public static int GetHighscore()
        {
            return _highscore;
        }

        public static int GetBirdsHit(BirdType type)
        {
            return _birdsHit[(int)type];
        }

        public static bool IsNewHighscore()
        {
            return _isNewHighscore;
        }
    }
}
