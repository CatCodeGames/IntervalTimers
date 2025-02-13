using System;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class RealtimeSingleInvokeProcessor : RealtimeProcessor
    {
        private static readonly ObjectPool<RealtimeSingleInvokeProcessor> _pool = new(() => new());

        public static RealtimeSingleInvokeProcessor Create(RealtimeTimerData data, Func<float> getTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(data, getTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - Data.LastTime;
            if (elapsedTime < Data.Interval)
                return true;

            return Data.TotalTicks < 0
                ? InfinitySingleInvoke(time)
                : LimitedSingleInvoke(time);
        }

        private bool InfinitySingleInvoke(float time)
        {
            Data.CompletedTicks++;
            Data.LastTime += Data.Interval;
            OnElapsed?.Invoke();
            return true;
        }

        private bool LimitedSingleInvoke(float time)
        {
            Data.CompletedTicks++;
            Data.LastTime += Data.Interval;
            OnElapsed?.Invoke();
            return Data.CompletedTicks < Data.TotalTicks;
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}