using System;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class DynamicRealtimeProcessor : RealtimeProcessor
    {
        private static readonly ObjectPool<DynamicRealtimeProcessor> _pool = new(() => new());

        public static DynamicRealtimeProcessor Create()
            => _pool.Get();

        public static DynamicRealtimeProcessor Create(RealtimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }

        private DynamicRealtimeProcessor() { }


        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var scheduledTime = TimerData.LastTime + TimerData.Interval;

            if (scheduledTime > time)
                return true;

            TickInfo.TicksCount = 0;
            while (scheduledTime < time)
            {
                TimerData.LastTime = scheduledTime;
                TickData.CompletedTicks++;
                TickInfo.TicksCount++;
                TickData.OnTick?.Invoke();

                if (!IsActive || (TickData.TotalTicks > 0 && TickData.CompletedTicks >= TickData.TotalTicks))
                    return false;

                scheduledTime += TimerData.Interval;
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