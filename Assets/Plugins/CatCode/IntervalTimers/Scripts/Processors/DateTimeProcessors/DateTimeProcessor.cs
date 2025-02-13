using System;

namespace CatCode.Timers
{
    public abstract class DateTimeProcessor : TimerProcessor
    {
        public Func<DateTime> GetTime;
        public DateTimeTimerData Data;

        protected void Init(DateTimeTimerData data, Func<DateTime> getTime, Action onElapsed, Action onFinished)
        {
            Data = data;
            GetTime = getTime;
            OnElapsed = onElapsed;
            OnFinished = onFinished;
        }

        protected void Reset()
        {
            Data = null;
            GetTime = null;
            OnElapsed = null;
            OnFinished = null;
        }
    }
}