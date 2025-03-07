using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class SingleInvokeDateTimeProcessor : DateTimeProcessor
    {
        private static readonly ObjectPool<SingleInvokeDateTimeProcessor> _pool = new(() => new());

        public static SingleInvokeDateTimeProcessor Create()
            => _pool.Get();

        public static SingleInvokeDateTimeProcessor Create(DateTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<DateTime> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }

        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - TimerData.LastTime;

            if (elapsedTime < TimerData.Interval)
                return true;

            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = (int)(elapsedTime / TimerData.Interval);
            var newCompletedTicks = completedTicks + ticks;

            var completed = false;
            if (totalTicks > 0 && newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                completed = true;
            }

            time = TimerData.LastTime + ticks * TimerData.Interval;

            TimerData.LastTime = time;
            TickData.CompletedTicks += ticks;
            TickInfo.TicksCount = ticks;

            TickData.OnTick?.Invoke();

            TickInfo.TicksCount = 0;

            return !completed;
        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }
}