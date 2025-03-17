using System;
using UnityEngine.Pool;
using UnityEngine;

namespace CatCode.Timers
{
    public sealed class SingleInvokeDeltaTimeProcessor : DeltaTimeProcessor
    {
        private static readonly ObjectPool<SingleInvokeDeltaTimeProcessor> _pool = new(() => new());

        public static SingleInvokeDeltaTimeProcessor Create()
            => _pool.Get();

        public static SingleInvokeDeltaTimeProcessor Create(DeltaTimeTimerData timerData, TimerTickData tickData, TimerTickInfo tickInfo, ref Func<float> getDeltaTime, Action onFinished)
        {
            var result = _pool.Get();
            result.Init(timerData, tickData, tickInfo, ref getDeltaTime, onFinished);
            return result;
        }


        private SingleInvokeDeltaTimeProcessor() { }

        protected override bool MoveNextCore()
        {
            var deltaTime = GetDeltaTime();
            TimerData.ElapsedTime += deltaTime;
            if (TimerData.ElapsedTime < TimerData.Interval)
                return true;

            var elapsedTime = TimerData.ElapsedTime;
            var interval = TimerData.Interval;
            var completedTicks = TickData.CompletedTicks;
            var totalTicks = TickData.TotalTicks;

            var ticks = Mathf.FloorToInt(elapsedTime / interval);
            var remainingTime = elapsedTime % interval;
            var newCompletedTicks = completedTicks + ticks;

            var result = true;
            if (totalTicks > 0 && newCompletedTicks >= totalTicks)
            {
                ticks = Mathf.Min(newCompletedTicks, totalTicks) - completedTicks;
                remainingTime = elapsedTime - ticks * interval;
                result = false;
            }

            TimerData.ElapsedTime = remainingTime;

            TickInfo.TicksCount = ticks;
            TickData.CompletedTicks += ticks;

            TickData.OnTick?.Invoke();

            TickInfo.TicksCount = 0;

            return result;

        }

        protected override void OnStopped()
        {
            Reset();
            _pool.Release(this);
        }
    }

}