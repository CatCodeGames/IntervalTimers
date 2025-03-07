using System;

namespace CatCode.Timers
{
    public abstract class RealtimeProcessor : TimerProcessor
    {
        public RealtimeTimerData TimerData;
        public Func<float> GetTime;

        public void Init(RealtimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getTime, Action onFinished)
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