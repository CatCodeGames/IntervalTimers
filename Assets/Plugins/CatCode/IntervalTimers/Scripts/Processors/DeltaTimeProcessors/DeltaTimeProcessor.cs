using System;

namespace CatCode.Timers
{
    public abstract class DeltaTimeProcessor : TimerProcessor
    {
        public Func<float> GetDeltaTime;
        public DeltaTimeTimerData Data;

        public void Init(DeltaTimeTimerData data, Func<float> getDeltaTime, Action onElapsed, Action onFinished)
        {
            Data = data;
            GetDeltaTime = getDeltaTime;
            OnElapsed = onElapsed;
            OnFinished = onFinished;
        }

        public void Reset()
        {
            Data = null;
            GetDeltaTime = null;
            OnElapsed = null;
            OnFinished = null;
        }
    }
}