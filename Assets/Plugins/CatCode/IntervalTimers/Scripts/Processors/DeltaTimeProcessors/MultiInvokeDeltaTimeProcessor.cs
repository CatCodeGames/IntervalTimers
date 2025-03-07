using System;
using UnityEngine.Pool;
using UnityEngine;

namespace CatCode.Timers
{
    public sealed class MultiInvokeDeltaTimeProcessor : DeltaTimeProcessor
    {
        private static readonly ObjectPool<MultiInvokeDeltaTimeProcessor> _pool = new(() => new());

        public static MultiInvokeDeltaTimeProcessor Create()
            => _pool.Get();

        public static MultiInvokeDeltaTimeProcessor Create(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getDeltaTime, onFinished);
            return result;
        }

        private MultiInvokeDeltaTimeProcessor() { }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            TimerData.ElapsedTime += deltaTime;
            if (TimerData.ElapsedTime < TimerData.Interval)
                return true;

            int ticks = Mathf.FloorToInt(TimerData.ElapsedTime / TimerData.Interval);
            var elapsedTime = TimerData.ElapsedTime;

            TickInfo.TicksCount = 0;
            for (int i = 0; i < ticks; i++)
            {
                TimerData.ElapsedTime = elapsedTime - TimerData.Interval * (i + 1);
                TickData.CompletedTicks++;
                TickInfo.TicksCount++;
                TickData.OnTick?.Invoke();

                if (!IsActive || (TickData.TotalTicks > 0 && TickData.CompletedTicks >= TickData.TotalTicks))
                {
                    TickInfo.TicksCount = 0;
                    return false;
                }
            }
            TickInfo.TicksCount = 0;
            return true;
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }

}