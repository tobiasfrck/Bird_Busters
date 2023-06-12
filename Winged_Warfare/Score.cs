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
        public static int HighScore;

        public static void IncreaseScore(int i)
        {
            CurrentScore += i;
        }
        public static void IncreaseScore()
        {
            CurrentScore += 1;
        }

    }
}
