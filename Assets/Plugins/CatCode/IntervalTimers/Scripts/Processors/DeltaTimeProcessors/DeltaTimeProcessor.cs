using System;

namespace CatCode.Timers
{

    public abstract class DeltaTimeProcessor : TimerProcessor
    {
        public DeltaTimeTimerData TimerData;
        public Func<float> GetDeltaTime;

        public void Init(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            TickData = tickData;
            TickInfo = tickInfo;
            TimerData = timerData;
            GetDeltaTime = getDeltaTime;
            OnFinished = onFinished;
        }

        protected void Reset()
        {
            TickData = null;
            TickInfo = null;
            TimerData = null;
            GetDeltaTime = null;
            OnFinished = null;
        }
    }
}