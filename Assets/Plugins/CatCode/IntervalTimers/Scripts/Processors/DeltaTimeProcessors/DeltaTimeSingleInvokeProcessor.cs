using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DeltaTimeSingleInvokeProcessor : DeltaTimeProcessor
    {
        private static readonly ObjectPool<DeltaTimeSingleInvokeProcessor> _pool = new(() => new());
        public static DeltaTimeSingleInvokeProcessor Create(DeltaTimeTimerData data, Func<float> getDeltaTime, Action onElapsed, Action onFinished)
        {
            var result = _pool.Get();            
            result.Init(data, getDeltaTime, onElapsed, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            Data.ElapsedTime += deltaTime;
            if (Data.ElapsedTime < Data.Interval)
                return true;

            return Data.TotalTicks < 0
                ? InfinitySingleInvoke(deltaTime)
                : LimitedSingleInvoke(deltaTime);
        }

        private bool InfinitySingleInvoke(float deltaTime)
        {
            Data.CompletedTicks++;
            OnElapsed?.Invoke();
            Data.ElapsedTime -= Mathf.Max(deltaTime, Data.Interval);
            return true;
        }

        private bool LimitedSingleInvoke(float deltaTime)
        {
            Data.CompletedTicks++;
            OnElapsed?.Invoke();
            Data.ElapsedTime -= Mathf.Max(deltaTime, Data.Interval);
            return Data.CompletedTicks < Data.TotalTicks;
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}
