using System;

namespace CatCode.Timers
{
    public abstract class RealtimeProcessor : TimerProcessor
    {
        public Func<float> GetTime;
        public RealtimeTimerData Data;

        protected void Init(RealtimeTimerData data, Func<float> getTime, Action onElapsed, Action onFinished)
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