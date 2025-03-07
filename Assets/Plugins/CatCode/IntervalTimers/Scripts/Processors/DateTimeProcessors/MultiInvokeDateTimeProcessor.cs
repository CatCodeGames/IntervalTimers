using System;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class MultiInvokeDateTimeProcessor : DateTimeProcessor
    {
        private static readonly ObjectPool<MultiInvokeDateTimeProcessor> _pool = new(() => new());

        public static MultiInvokeDateTimeProcessor Create()
            => _pool.Get();

        public static MultiInvokeDateTimeProcessor Create(DateTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<DateTime> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }

        private MultiInvokeDateTimeProcessor() { }

        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - TimerData.LastTime;

            if (elapsedTime < TimerData.Interval)
                return true;

            var lastTime = TimerData.LastTime;
            var interval = TimerData.Interval;
            var ticks = (int)(elapsedTime / interval);

            TickInfo.TicksCount = 0;
            for (int i = 0; i < ticks; i++)
            {
                TimerData.LastTime = lastTime + (i + 1) * interval;
                TickInfo.TicksCount = i + 1;
                TickData.CompletedTicks++;

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