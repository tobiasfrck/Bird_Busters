using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winged_Warfare
{
    public class Timer
    {
        public static float _fps;
        private int milliseconds;
        private int initMilliseconds;
        Delegate onTimerEnd;
        private bool isRunning = true;
        private bool isLooping = false;

        public Timer(int milliseconds)
        {
            this.milliseconds = milliseconds;
            this.initMilliseconds = milliseconds;
        }
        public Timer(int milliseconds, Delegate onTimerEnd)
        {
            this.milliseconds = milliseconds;
            this.initMilliseconds = milliseconds;
            this.onTimerEnd = onTimerEnd;
        }

        public void Update()
        {
            if (!isRunning) return;
            milliseconds -= (int)(1000/_fps);
            if (milliseconds <= 0)
            {
                onTimerEnd?.DynamicInvoke();
                if (isLooping)
                {
                    milliseconds = initMilliseconds;
                }
                else
                {
                    isRunning=false;
                }
            }
        }

        // Needs to be called if not looping
        public bool IsRunning()
        {
            return isRunning;
        }

        // Returns true if the timer was restarted; false if it was still running
        public bool RestartIfTimeElapsed()
        {
            if(isRunning) return false;
            milliseconds = initMilliseconds;
            isRunning = true;
            return true;
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void Continue()
        {
            isRunning = true;
        }

        public int GetSeconds()
        {
            return milliseconds / 1000;
        }
    }
}
