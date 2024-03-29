﻿using System;

namespace Bird_Busters
{
    public class Timer
    {
        public static float Fps;
        private float milliseconds;
        private int initMilliseconds;
        Delegate onTimerEnd;
        float delegateParam = -1;
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
        public Timer(int milliseconds, Delegate onTimerEnd, float param)
        {
            this.milliseconds = milliseconds;
            this.initMilliseconds = milliseconds;
            this.onTimerEnd = onTimerEnd;
            this.delegateParam = param;
        }

        public void Update()
        {
            if (!isRunning) return;
            milliseconds -= (1000 / Fps);
            if (milliseconds <= 0)
            {
                if (onTimerEnd != null)
                {
                    if (delegateParam.Equals(-1))
                    {
                        onTimerEnd.DynamicInvoke();
                    }
                    else
                    {
                        onTimerEnd.DynamicInvoke(delegateParam);
                    }
                }
                if (isLooping)
                {
                    milliseconds = initMilliseconds;
                }
                else
                {
                    isRunning = false;
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
            if (isRunning) return false;
            milliseconds = initMilliseconds;
            isRunning = true;
            return true;
        }

        // Returns true if the timer was restarted and set to the new time; false if it was still running
        public bool RestartIfTimeElapsed(int milliseconds)
        {
            if (isRunning) return false;
            this.milliseconds = milliseconds;
            this.initMilliseconds = milliseconds;
            isRunning = true;
            return true;
        }

        public void SetTimeNRun(int milliseconds)
        {
            this.milliseconds = milliseconds;
            this.initMilliseconds = milliseconds;
            isRunning = true;
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void Continue()
        {
            isRunning = true;
        }

        public bool HasReached(int milliseconds)
        {
            return this.milliseconds <= milliseconds;
        }

        public int GetSeconds()
        {
            return (int)Math.Round(milliseconds / 1000, 0, MidpointRounding.AwayFromZero);
        }

        public float GetProgress()
        {
            return (initMilliseconds - milliseconds) / initMilliseconds;
        }

    }
}
