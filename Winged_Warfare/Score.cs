using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winged_Warfare
{
    public class Score
    {
        private static int _currentScore = 0;
        private static int _highscore;

        public static void IncreaseScore(int i)
        {
            _currentScore += i;
            if (_currentScore > _highscore)
            {
                SetHighscore(_currentScore);
            }
        }
        public static void IncreaseScore()
        {
            IncreaseScore(1);
        }

        public static void ResetScore()
        {
            _currentScore = 0;
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
    }
}
