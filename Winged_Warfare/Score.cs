using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winged_Warfare
{
    public class Score
    {
        public static int CurrentScore = 0;
        private static int _maxScore = 10; // TODO: Score should be based on time, not on kills, this is just to have a gameplay-loop
        public static int HighScore;

        public static void IncreaseScore(int i)
        {
            CurrentScore += i;
        }
        public static void IncreaseScore()
        {
            CurrentScore += 1;
        }

        public static void ResetScore()
        {
            CurrentScore = 0;
        }

        public static bool HasWon()
        {
            if (CurrentScore >= _maxScore)
            {
                return true;
            }
            return false;
        }

    }
}
