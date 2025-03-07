using System;

namespace CatCode.Timers
{
    public abstract class DateTimeProcessor : TimerProcessor
    {
        public DateTimeTimerData TimerData;
        public Func<DateTime> GetTime;

        public void Init(DateTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<DateTime> getTime, Action onFinished)
        {
            TickData = tickData;
            TickInfo = tickInfo;
            TimerData = timerData;
            GetTime = getTime;
            OnFinished = onFinished;
        }

        protected void Reset()
        {
            TickData = null;
            TickInfo = null;
            TimerData = null;
            GetTime = null;
            OnFinished = null;
        }
    }
}