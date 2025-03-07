using System;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DynamicDeltaTimeProcessor : DeltaTimeProcessor
    {
        private static readonly ObjectPool<DynamicDeltaTimeProcessor> _pool = new(() => new());

        public static DynamicDeltaTimeProcessor Create()
            => _pool.Get();

        public static DynamicDeltaTimeProcessor Create(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getDeltaTime, onFinished);
            return result;
        }

        private DynamicDeltaTimeProcessor() { }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            TimerData.ElapsedTime += deltaTime;
            if (TimerData.ElapsedTime < TimerData.Interval)
                return true;
            TickInfo.TicksCount = 0;
            while (TimerData.ElapsedTime >= TimerData.Interval)
            {
                TickInfo.TicksCount++;
                TimerData.ElapsedTime -= TimerData.Interval;
                TickData.CompletedTicks++;
                TickData.OnTick?.Invoke();

                if (!IsActive || (TickData.TotalTicks >0 && TickData.CompletedTicks >= TickData.TotalTicks))
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