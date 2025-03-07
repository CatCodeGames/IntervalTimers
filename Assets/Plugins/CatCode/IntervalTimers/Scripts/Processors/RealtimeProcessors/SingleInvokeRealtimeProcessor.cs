using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.Timers
{
    public sealed class SingleInvokeRealtimeProcessor : RealtimeProcessor
    {
        private static readonly ObjectPool<SingleInvokeRealtimeProcessor> _pool = new(() => new());

        public static SingleInvokeRealtimeProcessor Create()
            => _pool.Get();

        public static SingleInvokeRealtimeProcessor Create(RealtimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getTime, onFinished);
            return result;
        }

        private SingleInvokeRealtimeProcessor() { }


        protected override bool MoveNextCore()
        {
            var time = GetTime();
            var elapsedTime = time - TimerData.LastTime;

            if (elapsedTime < TimerData.Interval)
                return true;

            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / TimerData.Interval);
            var newCompletedTicks = completedTicks + ticks;

            var completed = false;
            if (totalTicks > 0 && newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                completed = true;
            }

            time = TimerData.LastTime + ticks * TimerData.Interval;

            TimerData.LastTime = time;
            TickInfo.TicksCount = ticks;
            TickData.CompletedTicks += ticks;

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